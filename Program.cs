using System.Numerics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/task3/billalhossain.bhj@gmail.com", (string? x, string? y) =>
{    
    if(!int.TryParse(x, out var a ) | !int.TryParse(y, out var b ))
    {
        return "NaN";
    }

    static int gdc(int a, int b)
    {
        while(b!=0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    int lcm = (a/ gdc(a,b)) * b;

    return lcm.ToString();
});

app.Run();
