using System.Globalization;

namespace TollFeeCalculator;

public static class CommandLineParser
{
    public static TollCalculatorInput ParseInput()
    {
        Console.WriteLine("Welcome to VGR Toll Fee Calculator!");

        Console.WriteLine("Please enter the vehicle type");
        var vehicleType = GetVehicleType();

        Console.WriteLine(
            "Please enter the dates and times of all passes during a single day. End with an empty line."
        );
        var dates = GetDates();

        return new TollCalculatorInput(vehicleType, [.. dates]);
    }

    private static VehicleTypes GetVehicleType()
    {
        VehicleTypes vehicleType;
        while (true)
        {
            var vehicleTypeInput = Console.ReadLine();
            if (Enum.TryParse(vehicleTypeInput, true, out vehicleType))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid vehicle type. Please try again.");
            }
        }

        return vehicleType;
    }

    private static List<DateTime> GetDates()
    {
        List<DateTime> dates = new();

        while (true)
        {
            var dateInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dateInput))
            {
                break;
            }

            if (
                DateTime.TryParseExact(
                    dateInput,
                    "yyyy-MM-dd HH.mm",
                    CultureInfo.GetCultureInfo("sv-SE"),
                    DateTimeStyles.None,
                    out var date
                )
            )
            {
                if (dates.Count > 0 && date.Date != dates[0].Date)
                {
                    Console.WriteLine("All passes must be on the same day. Please try again.");
                    continue;
                }

                dates.Add(date);
            }
            else
            {
                Console.WriteLine("Invalid date. Please try again.");
            }
        }
        return dates;
    }
}
