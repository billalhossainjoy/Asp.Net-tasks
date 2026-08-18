using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Asp.Net_tasks.Models;
using Microsoft.EntityFrameworkCore;
using Asp.Net_tasks.ViewModel.Home;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace Asp.Net_tasks.Controllers;

[Authorize]
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
    if (selectedIds.Count == 0)
    {
        TempData["ErrorMessage"] =
            "Please select at least one user.";

        return RedirectToAction(nameof(Index));
    }

    var users = await _db.Users
        .Where(x => selectedIds.Contains(x.Id))
        .ToListAsync();

    foreach (var user in users)
    {
        user.Status = UserStatus.Blocked;
    }

    await _db.SaveChangesAsync();

    TempData["SuccessMessage"] =
        $"{users.Count} user(s) blocked successfully.";

    return RedirectToAction(nameof(Index));
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UnBlock(
    List<Guid> selectedIds)
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

    foreach (var user in users)
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
public async Task<IActionResult> Delete(
    List<Guid> selectedIds)
{
    if (selectedIds.Count == 0)
    {
        TempData["ErrorMessage"] =
            "Please select at least one user.";

        return RedirectToAction(nameof(Index));
    }

    var deletedCount = await _db.Users
        .Where(x => selectedIds.Contains(x.Id))
        .ExecuteDeleteAsync();

    TempData["SuccessMessage"] =
        $"{deletedCount} user(s) deleted successfully.";

    return RedirectToAction(nameof(Index));
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteUnverified()
{
    var deletedCount = await _db.Users
        .Where(x => x.Status == UserStatus.Unverified)
        .ExecuteDeleteAsync();

    if (deletedCount == 0)
    {
        TempData["ErrorMessage"] =
            "There are no unverified users to delete.";

        return RedirectToAction(nameof(Index));
    }

    TempData["SuccessMessage"] =
        $"{deletedCount} unverified user(s) deleted successfully.";

    return RedirectToAction(nameof(Index));
}



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
