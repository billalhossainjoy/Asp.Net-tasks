

using Microsoft.AspNetCore.Mvc;

namespace Asp.Net_task3.Controllers;

public class AuthController : Controller
{
    public IActionResult Login()
    {
        return View();
    }
    public IActionResult Register()
    {
        return View();
    }
}