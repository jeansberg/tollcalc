using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TollFeeCalculator;

namespace tollcalc.tests;

public class TollCalculatorTests
{
    private readonly IConfiguration _config;

    public TollCalculatorTests()
    {
        _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }

    [Fact]
    public void GetTollFee_ShouldReturnFee_ForSinglePass()
    {
        var sut = CreateTollCalculator();

        sut.GetTollFee(VehicleTypes.Car, [new DateTime(2013, 1, 3, 08, 00, 00)]).Should().Be(13);
    }

    [Fact]
    public void GetTollFee_ShouldReturnHighestFee_ForMultiplePassesInOneHour()
    {
        var sut = CreateTollCalculator();

        sut.GetTollFee(
                VehicleTypes.Car,
                [new DateTime(2013, 1, 3, 08, 00, 00), new DateTime(2013, 1, 3, 08, 45, 00)]
            )
            .Should()
            .Be(13);
    }

    [Fact]
    public void GetTollFee_ShouldReturnCumulativeFee_ForPassesDuringMultipleHours()
    {
        var sut = CreateTollCalculator();

        sut.GetTollFee(
                VehicleTypes.Car,
                [
                    new DateTime(2013, 1, 3, 08, 00, 00),
                    new DateTime(2013, 1, 3, 08, 45, 00),
                    new DateTime(2013, 1, 3, 10, 00, 00),
                    new DateTime(2013, 1, 3, 12, 00, 00),
                ]
            )
            .Should()
            .Be(29);
    }

    [Fact]
    public void GetTollFee_ShouldReturnCumulativeFee_ForPassesDuringMultipleHoursCappedAtMax()
    {
        TollCalculatorOptions options = new();
        _config.GetSection(nameof(TollCalculatorOptions)).Bind(options);

        var sut = new TollCalculator(Options.Create(options));

        sut.GetTollFee(
                VehicleTypes.Car,
                [
                    new DateTime(2013, 1, 3, 07, 00, 00),
                    new DateTime(2013, 1, 3, 08, 00, 00),
                    new DateTime(2013, 1, 3, 09, 00, 00),
                    new DateTime(2013, 1, 3, 10, 00, 00),
                    new DateTime(2013, 1, 3, 11, 00, 00),
                    new DateTime(2013, 1, 3, 12, 00, 00),
                    new DateTime(2013, 1, 3, 13, 00, 00),
                    new DateTime(2013, 1, 3, 14, 00, 00),
                    new DateTime(2013, 1, 3, 15, 00, 00),
                    new DateTime(2013, 1, 3, 16, 00, 00),
                    new DateTime(2013, 1, 3, 17, 00, 00),
                    new DateTime(2013, 1, 3, 18, 00, 00),
                ]
            )
            .Should()
            .Be(options.MaxDailyFee);
    }

    [Theory]
    [InlineData(2013, 1, 5)]
    [InlineData(2013, 1, 6)]
    public void GetTollFee_ShouldReturnZero_ForTollFreeDayOfWeek(int year, int month, int day)
    {
        var sut = CreateTollCalculator();

        sut.GetTollFee(VehicleTypes.Car, [new DateTime(year, month, day, 00, 00, 00)])
            .Should()
            .Be(0);
    }

    [Theory]
    [InlineData(2013, 1, 1)]
    [InlineData(2013, 3, 28)]
    [InlineData(2013, 3, 29)]
    [InlineData(2013, 4, 1)]
    [InlineData(2013, 4, 30)]
    [InlineData(2013, 5, 1)]
    [InlineData(2013, 5, 8)]
    [InlineData(2013, 5, 9)]
    [InlineData(2013, 6, 5)]
    [InlineData(2013, 6, 6)]
    [InlineData(2013, 6, 21)]
    [InlineData(2013, 7, 1)]
    [InlineData(2013, 11, 1)]
    [InlineData(2013, 12, 24)]
    [InlineData(2013, 12, 25)]
    [InlineData(2013, 12, 26)]
    [InlineData(2013, 12, 31)]
    public void GetTollFee_ShouldReturnZero_ForTollFreeDate(int year, int month, int day)
    {
        var sut = CreateTollCalculator();

        sut.GetTollFee(VehicleTypes.Car, [new DateTime(year, month, day, 00, 00, 00)])
            .Should()
            .Be(0);
    }

    [Theory]
    [InlineData(VehicleTypes.Motorbike)]
    [InlineData(VehicleTypes.Tractor)]
    [InlineData(VehicleTypes.Emergency)]
    [InlineData(VehicleTypes.Diplomat)]
    [InlineData(VehicleTypes.Foreign)]
    [InlineData(VehicleTypes.Military)]
    public void GetTollFee_ShouldReturnZero_ForTollFreeVehicle(VehicleTypes vehicle)
    {
        var sut = CreateTollCalculator();

        sut.GetTollFee(vehicle, [new DateTime(2013, 1, 3, 08, 00, 00)]).Should().Be(0);
    }

    private TollCalculator CreateTollCalculator()
    {
        TollCalculatorOptions options = new();
        _config.GetSection(nameof(TollCalculatorOptions)).Bind(options);

        var sut = new TollCalculator(Options.Create(options));
        return sut;
    }
}
