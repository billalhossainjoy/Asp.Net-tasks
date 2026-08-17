

using System.Security.Claims;
using Asp.Net_task3.ViewModel.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net_task3.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    private async void ClaimUserAsync(User user, bool rememberMe = false)
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

    public AuthController(AppDbContext db, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
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
        }

        ClaimUserAsync(user);
        user.LastLoginAt = DateTimeOffset.UtcNow;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Internal server error");
        }

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
            CreatedAt = DateTimeOffset.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _db.Users.Add(user);

        ClaimUserAsync(user);
        user.LastLoginAt = DateTimeOffset.UtcNow;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Internal server error");
        }

        TempData["SuccessMessage"] = "Registration successfully.";
        return RedirectToAction("Index", "Home");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Response.Redirect("/Account/Login");

        return RedirectToAction("Login", "Auth");
    }
}