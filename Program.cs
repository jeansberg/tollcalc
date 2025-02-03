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
    // Use the calculator instance as needed

    var result = calculator.GetTollFee(
        VehicleTypes.Car,
        [
            new DateTime(2013, 1, 1, 8, 10, 0),
            new DateTime(2013, 1, 1, 6, 0, 0),
            new DateTime(2013, 1, 1, 6, 20, 0),
            new DateTime(2013, 1, 1, 7, 10, 0),
        ]
    );
}

app.Run();
