using System.Numerics;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/app/billalhossain_bhj_gmail_com", (string? x, string? y) =>
{
    if (!BigInteger.TryParse(x, NumberStyles.Integer, CultureInfo.InvariantCulture, out var a) ||
        !BigInteger.TryParse(y, NumberStyles.Integer, CultureInfo.InvariantCulture, out var b))
    {
        return "NaN";
    }

    var lcm = a.IsZero || b.IsZero
        ? BigInteger.Zero
        : BigInteger.Abs(a / BigInteger.GreatestCommonDivisor(a, b) * b);

    return lcm.ToString(CultureInfo.InvariantCulture);
});

app.Run();
