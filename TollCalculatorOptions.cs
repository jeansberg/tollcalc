using TollFeeCalculator;

public sealed class TollCalculatorOptions
{
    public TollCalculatorOptions()
    {
        TollFeeLevels = new List<TollFeeLevel>();
        TollFreeVehicles = new List<TollFreeVehicles>();
        TollFreeDates = new List<DateTime>();
        TollFreeDaysOfWeek = new List<DayOfWeek>();
    }

    public List<TollFeeLevel> TollFeeLevels { get; set; }
    public List<TollFreeVehicles> TollFreeVehicles { get; set; }
    public List<DateTime> TollFreeDates { get; set; }
    public List<DayOfWeek> TollFreeDaysOfWeek { get; set; }
}
