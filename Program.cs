using Asp.Net_task3.Services.Email;
using Asp.Net_tasks.Services.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Mail service
builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection("smtp")
);

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton<IEmailQueue, EmailQueue>();
builder.Services.AddHostedService<EmailBackgroundService>();


// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(option =>
{
    option.LoginPath = "/auth/login";
    option.AccessDeniedPath = "/auth/login";

    option.Cookie.Name = "User.Auth";
    option.Cookie.HttpOnly= true;
    option.Cookie.SameSite= SameSiteMode.Lax;

    option.ExpireTimeSpan = TimeSpan.FromHours(10);
    option.SlidingExpiration = true;
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
