using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TollFeeCalculator;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.Sources.Clear();

IHostEnvironment env = builder.Environment;

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<TollCalculatorOptions>(
    builder.Configuration.GetSection(nameof(TollCalculatorOptions))
);

builder.Services.AddSingleton<TollCalculator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var calculator = services.GetRequiredService<TollCalculator>();

    var input = CommandLineParser.ParseInput();

    var result = calculator.GetTollFee(input.VehicleType, input.Dates);

    Console.WriteLine($"The total fee for the day is {result} kr");
}

app.Run();
