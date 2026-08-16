using System.Numerics;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/app", () =>
{
    return "billal"
});

app.Run();
