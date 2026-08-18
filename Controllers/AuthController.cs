using System.Security.Claims;
using Asp.Net_tasks.ViewModel.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Asp.Net_task3.Services.Email;

namespace Asp.Net_tasks.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailQueue _emailQueue;

    private async Task ClaimUserAsync(User user, bool rememberMe = false)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties
        {
            IsPersistent = rememberMe
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
         properties);

    }

    public AuthController(AppDbContext db, IPasswordHasher<User> passwordHasher, IEmailQueue emailQueue)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _emailQueue = emailQueue;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid email or password."
            );

            return View(model);
        }

        if (user.Status == UserStatus.Blocked)
        {
            ModelState.AddModelError(string.Empty, "Your account is blocked.");

            return View(model);
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Internal server error");
            return View(model);
        }

        await ClaimUserAsync(user);

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();

        var emailExists = await _db.Users.AnyAsync(x => x.Email == email);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Email already exists");
            return View(model);
        }

        var user = new User
        {
            Name = model.Name.Trim(),
            Email = email,
            Status = UserStatus.Unverified,
            CreatedAt = DateTimeOffset.UtcNow,
            EmailConfirmationToken = GenerateConfirmationToken(),

            EmailConfirmationTokenExpiresAtUtc =
                DateTimeOffset.UtcNow.AddHours(24)
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Internal server error");

            return View(model);
        }

        var confirmationUrl = Url.Action(
        "ConfirmEmail",
        "Auth",
        new
        {
            userId = user.Id,
            token = user.EmailConfirmationToken
        },
        Request.Scheme);

    await _emailQueue.QueueAsync(VerificationTemplate(user, confirmationUrl));

        TempData["SuccessMessage"] =
    "Registration successful. Please check your email.";
        return RedirectToAction("Login", "Auth");
    }

    private EmailMessage VerificationTemplate(
    User user,
    string confirmationUrl)
{
    var body = EmailTemplateBuilder.Build(
        title: "Confirm your email",
        greetingName: user.Name,
        message: "Thanks for registering. Please confirm your email address.",
        buttonText: "Verify Email",
        buttonUrl: confirmationUrl,
        note: "This verification link expires in 24 hours.");

    return new EmailMessage(
        user.Email,
        "Verify your email address",
        body);
}


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login", "Auth");
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
    {
        var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == userId);
        if(user is null) {
            return NotFound();
        }

        if(user.EmailConfirmationToken != token)
        {
            return BadRequest("Invalid Session.");
        }

        if(user.EmailConfirmationTokenExpiresAtUtc is null || user.EmailConfirmationTokenExpiresAtUtc < DateTimeOffset.UtcNow)
        {
            return BadRequest("Confirmation link has expired.");
        }

        if (user.Status != UserStatus.Blocked)
        {
            user.Status = UserStatus.Active;
        }

        user.EmailConfirmationToken = null;
        user.EmailConfirmationTokenExpiresAtUtc = null;
        await _db.SaveChangesAsync();

        
        TempData["SuccessMessage"] = "Your message has been confirmed.";

        return RedirectToAction("Index", "Home");
    }

    private static string GenerateConfirmationToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return WebEncoders.Base64UrlEncode(bytes);
    }
}