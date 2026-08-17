

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

public sealed class CurrentValidationMiddleware
{
    private readonly RequestDelegate _next;
    public CurrentValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        AppDbContext db
    )
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(id, out var userId))
            {
                await RejectAuthentication(context);
                return;
            }

            var user = await db.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new
            {
                x.Id,
                x.Status
            }).SingleOrDefaultAsync();

            if (user is null || user.Status == UserStatus.Blocked)
            {
                await RejectAuthentication(context);
                return;
            }
        }
        await _next(context);
    }

    private async Task RejectAuthentication(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        context.Response.Redirect("/Account/Login");

        return;
    }

}