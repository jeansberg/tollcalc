using TollFeeCalculator;

public sealed class TollCalculatorOptions
{
    public TollCalculatorOptions()
    {
        TollFeeLevels = new List<TollFeeLevel>();
        TollFreeVehicles = new List<VehicleTypes>();
        TollFreeDates = new List<DateTime>();
        TollFreeDaysOfWeek = new List<DayOfWeek>();
    }

    public int CalculationTimeSpanInMinutes { get; set; }
    public int MaxDailyFee { get; set; }
    public List<TollFeeLevel> TollFeeLevels { get; set; }
    public List<VehicleTypes> TollFreeVehicles { get; set; }
    public List<DateTime> TollFreeDates { get; set; }
    public List<DayOfWeek> TollFreeDaysOfWeek { get; set; }
}
