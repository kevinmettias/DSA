using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for FlightWorkloads (ARCHITECTURE 17.7). Two things feed the LC 787 reading and
// both live here: the K-stop bound the naive arm's search depth scales with, and the int[][]
// flight network every city can be reached from city 0 in.
public sealed partial class FlightWorkloadsTests
{
    private const int CityCount = 18;
    private const int Seed = 787; // LC problem number
    private const int FlightFieldCount = 3; // FromCity, ToCity, Price
    private const int MinFlightPrice = 1;
    private const int FlightPriceUpperBound = 100; // exclusive
    private const int ExtraFlightsPerCity = 2;
    private const int MinimumStopCount = 2;
    private const int StopScaleFactor = 6;
    private const int CityCountBelowTheScaleFactor = 6;

    // Below the scaling threshold the divisor would round the bound away, so the floor is what
    // keeps a small network's search depth non-trivial.
    [Fact]
    public void StopBound_CityCountBelowTheScaleFactor_IsClampedToTheMinimumStopCount() =>
        Assert.Equal(MinimumStopCount, FlightWorkloads.StopBound(CityCountBelowTheScaleFactor));

    [Fact]
    public void StopBound_CityCountAboveTheScaleFactor_ReturnsTheScaledStopCount()
    {
        const int LargeCityCount = 60;

        Assert.True(LargeCityCount / StopScaleFactor > MinimumStopCount);
        Assert.Equal(LargeCityCount / StopScaleFactor, FlightWorkloads.StopBound(LargeCityCount));
    }

    [Fact]
    public void BuildFlights_EveryFlight_IsAThreeFieldRowWithinTheCityRangeAndPriceBand()
    {
        var flights = FlightWorkloads.BuildFlights(CityCount, Seed);

        Assert.All(flights, flight => Assert.Equal(FlightFieldCount, flight.Length));
        Assert.All(flights, flight => Assert.InRange(flight[0], 0, CityCount - 1));
        Assert.All(flights, flight => Assert.InRange(flight[1], 0, CityCount - 1));
        Assert.All(flights, flight => Assert.InRange(flight[2], MinFlightPrice, FlightPriceUpperBound - 1));
    }

    // The generator writes a city's guaranteed flight as { earlier city, city, price }, and it is
    // that forward direction which makes every city reachable from city 0.
    [Fact]
    public void BuildFlights_EveryCityBeyondTheFirst_HasAnIncomingFlightFromAnEarlierCity()
    {
        var flights = FlightWorkloads.BuildFlights(CityCount, Seed);

        foreach (var city in Enumerable.Range(1, CityCount - 1))
        {
            Assert.Contains(flights, flight => flight[1] == city && flight[0] < city);
        }
    }

    [Fact]
    public void BuildFlights_FlightCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var flights = FlightWorkloads.BuildFlights(CityCount, Seed);

        Assert.InRange(flights.Length, CityCount - 1, CityCount - 1 + (CityCount * ExtraFlightsPerCity));
    }

    [Fact]
    public void BuildFlights_SameSeed_ReturnsTheSameNetwork() =>
        Assert.Equal(
            AnswerText.Of(FlightWorkloads.BuildFlights(CityCount, Seed)),
            AnswerText.Of(FlightWorkloads.BuildFlights(CityCount, Seed)));
}
