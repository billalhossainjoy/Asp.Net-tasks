using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Asp.Net_tasks.Models;
using Microsoft.EntityFrameworkCore;
using Asp.Net_tasks.ViewModel.Home;

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



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
