using Microsoft.Extensions.Options;

namespace TollFeeCalculator;

public class TollCalculator
{
    /**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     */

    private readonly TollCalculatorOptions options;

    public TollCalculator(IOptions<TollCalculatorOptions> options)
    {
        this.options = options.Value;
    }

    public int GetTollFee(Vehicle vehicle, DateTime[] dates)
    {
        DateTime intervalStart = dates[0];
        int totalFee = 0;
        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(date, vehicle);
            int tempFee = GetTollFee(intervalStart, vehicle);

            long diffInMillies = date.Millisecond - intervalStart.Millisecond;
            long minutes = diffInMillies / 1000 / 60;

            if (minutes <= 60)
            {
                if (totalFee > 0)
                    totalFee -= tempFee;
                if (nextFee >= tempFee)
                    tempFee = nextFee;
                totalFee += tempFee;
            }
            else
            {
                totalFee += nextFee;
            }
        }
        if (totalFee > 60)
            totalFee = 60;
        return totalFee;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
            return false;
        String vehicleType = vehicle.GetVehicleType();
        return options.TollFreeVehicles.Contains(Enum.Parse<VehicleTypes>(vehicleType));
    }

    public int GetTollFee(DateTime date, Vehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle))
            return 0;

        return
            options.TollFeeLevels.SingleOrDefault(t => HasMatchingTollFee(date, t))
                is TollFeeLevel tollFeeLevel
            ? tollFeeLevel.Fee
            : 0;
    }

    private static bool HasMatchingTollFee(DateTime date, TollFeeLevel t)
    {
        return date.Hour >= t.HourRangeStart
            && date.Hour <= t.HourRangeEnd
            && date.Minute >= t.MinuteRangeStart
            && date.Minute <= t.MinuteRangeEnd;
    }

    private Boolean IsTollFreeDate(DateTime date)
    {
        if (options.TollFreeDaysOfWeek.Contains(date.DayOfWeek))
            return true;

        if (options.TollFreeDates.Contains(date.Date))
        {
            return true;
        }
        return false;
    }
}
