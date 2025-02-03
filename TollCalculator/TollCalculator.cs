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

    public int GetTollFee(VehicleTypes vehicle, DateTime[] dates)
    {
        if (dates.Length == 0)
        {
            return 0;
        }

        var orderedDates = dates.OrderBy(d => d).ToArray();
        var groupedDates = GetDateTimeGroupsByOverlap(orderedDates);

        int totalFee = 0;

        foreach (var group in groupedDates)
        {
            var fee = CalculateTollFeeForGroup(group, vehicle);
            totalFee += fee;
        }

        return Math.Min(totalFee, 60);
    }

    private int CalculateTollFeeForGroup(List<DateTime> group, VehicleTypes vehicle)
    {
        var highestFee = 0;
        foreach (var date in group)
        {
            var fee = GetTollFee(date, vehicle);
            if (fee > highestFee)
            {
                highestFee = fee;
            }
        }

        return highestFee;
    }

    private static List<List<DateTime>> GetDateTimeGroupsByOverlap(DateTime[] orderedDates)
    {
        var groupedDatesByOverlapPeriod = new List<List<DateTime>>();
        var currentGroup = new List<DateTime> { };
        for (int i = 0; i < orderedDates.Length; i++)
        {
            if (i == 0)
            {
                currentGroup.Add(orderedDates[i]);
                groupedDatesByOverlapPeriod.Add(currentGroup);
            }
            else
            {
                var firstDateInGroup = currentGroup[0];
                var currentDate = orderedDates[i];

                if (currentDate.Subtract(firstDateInGroup).TotalMinutes <= 60)
                {
                    groupedDatesByOverlapPeriod.Last().Add(currentDate);
                }
                else
                {
                    var newGroup = new List<DateTime> { currentDate };
                    groupedDatesByOverlapPeriod.Add(newGroup);
                    currentGroup = newGroup;
                }
            }
        }

        return groupedDatesByOverlapPeriod;
    }

    private bool IsTollFreeVehicle(VehicleTypes vehicle)
    {
        return options.TollFreeVehicles.Contains(vehicle);
    }

    public int GetTollFee(DateTime date, VehicleTypes vehicle)
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
        var timeSpanStart = new TimeSpan(t.HourRangeStart, t.MinuteRangeStart, 0);
        var timeSpanEnd = new TimeSpan(t.HourRangeEnd, t.MinuteRangeEnd, 0);
        return date.TimeOfDay >= timeSpanStart && date.TimeOfDay <= timeSpanEnd;
    }

    private bool IsTollFreeDate(DateTime date)
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
