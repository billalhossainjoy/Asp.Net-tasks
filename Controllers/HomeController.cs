using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Asp.Net_tasks.Models;
using Microsoft.EntityFrameworkCore;
using Asp.Net_tasks.ViewModel.Home;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Asp.Net_tasks.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db= db;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _db.Users.AsNoTracking().OrderByDescending(x => x.LastLoginAt).Select(x => new UserListItemViewModel
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Email,
            Status = x.Status,
            LastLoginAt = x.LastLoginAt
        }).ToListAsync();
        return View(users);
    }

[HttpPost]
[ValidateAntiForgeryToken]
    public async Task<IActionResult> Block(List<Guid> selectedIds)
    {
   var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

   if (!Guid.TryParse(userId, out var currentUserId))
        {
            return Unauthorized();
        }

        if (selectedIds.Count == 0)
        {
        TempData["ErrorMessage"] = "Please select at least one user.";
            return RedirectToAction(nameof(Index));
        }

        var users = await _db.Users.Where(x => selectedIds.Contains(x.Id)).ToListAsync();

        foreach (var user in users)
            {
                if (user.Id == currentUserId)
                {
                    continue;
                }
                user.Status = UserStatus.Blocked;
            }


    await _db.SaveChangesAsync();


    TempData["SuccessMessage"] =
        $"{users.Count} user(s) blocked successfully.";


        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnBlock(List<Guid> selectedIds)
    {
    if (selectedIds.Count == 0)
    {
        TempData["ErrorMessage"] =
            "Please select at least one user.";

        return RedirectToAction(nameof(Index));
    }

        var users = _db.Users.Where(x => selectedIds.Contains(x.Id)).ToList();


        foreach(var user in users)
        {
            user.Status = UserStatus.Active;
        }

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
        $"{users.Count} user(s) unblocked successfully.";

        return RedirectToAction(nameof(Index));
    }

[HttpPost]
[ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete (List<Guid> selectedIds)
    {
    if (selectedIds.Count == 0)
    {
        TempData["ErrorMessage"] =
            "Please select at least one user.";

        return RedirectToAction(nameof(Index));
    }

        var users = await _db.Users
        .Where(x => selectedIds.Contains(x.Id))
        .ToListAsync();


    if (users.Count == 0)
    {
        TempData["ErrorMessage"] =
            "No users were found.";

        return RedirectToAction(nameof(Index));
    }


    _db.Users.RemoveRange(users);

    await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
        $"{users.Count} user(s) deleted successfully.";


        return RedirectToAction(nameof(Index));
    }





    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
