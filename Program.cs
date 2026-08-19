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

// Resend's HTTPS API works on hosting plans that block outbound SMTP ports.
// Continue using the application's existing Smtp configuration variables.
builder.Services.AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection("Smtp"))
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Password),
        "Smtp__Password is required.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.FromEmail),
        "Smtp__FromEmail is required.")
    .ValidateOnStart();

builder.Services.AddHttpClient<IEmailSender, EmailSender>(client =>
{
    client.BaseAddress = new Uri("https://api.resend.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
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

// Apply pending EF Core migrations when the container starts. This keeps a
// newly provisioned production database in sync with the application.
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<CurrentValidationMiddleware>();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
