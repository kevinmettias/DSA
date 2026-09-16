using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for TimedRoadWorkloads (ARCHITECTURE 17.7): LC 1928 searches a two-way road map for the
// cheapest route inside a minute budget. The fixture's documented shape is a chain where every city reaches
// the next in one minute and the one after in two, with fees cycling so no route is uniformly cheapest and a
// budget that is exactly what a straight run costs - so the naive walk has to discover that doubling back
// costs more minutes than it has rather than being handed a one-way map.
public sealed partial class TimedRoadWorkloadsTests
{
    private const int LastCity = 16;
    private const int UnitStepSize = 1;
    private const int LongStepSize = 2;
    private const int RoadWidth = 3; // FromCity, ToCity, Minutes
    private const int MinutesFieldIndex = 2;
    private const int FeeCycleLength = 7;
    private const int SmallestFee = 1;
    private const int LargestFee = 7;

    [Fact]
    public void BuildStepChain_Chain_SpendsExactlyTheCityDistanceOnEveryRoad()
    {
        var roads = TimedRoadWorkloads.BuildStepChain(LastCity);

        Assert.NotEmpty(roads);
        Assert.All(roads, road => Assert.Equal(RoadWidth, road.Length));
        Assert.All(roads, road => Assert.InRange(road[1] - road[0], UnitStepSize, LongStepSize));
        Assert.All(roads, road => Assert.Equal(road[1] - road[0], road[MinutesFieldIndex]));
        Assert.All(roads, road => Assert.InRange(road[0], 0, LastCity - 1));
    }

    // Two roads out of every city - the short step and the long one - which is what makes the number of
    // distinct routes genuinely large rather than a single corridor.
    [Fact]
    public void BuildStepChain_EveryCityButTheLast_LeavesAShortAndALongRoad()
    {
        var roads = TimedRoadWorkloads.BuildStepChain(LastCity);

        Assert.All(
            Enumerable.Range(0, LastCity),
            city => Assert.Contains(roads, road => road[0] == city && road[1] - road[0] == UnitStepSize));
        Assert.All(
            Enumerable.Range(0, LastCity - LongStepSize + 1),
            city => Assert.Contains(roads, road => road[0] == city && road[1] - road[0] == LongStepSize));
    }

    [Fact]
    public void BuildCyclingFees_EveryCity_GetsAFeeInTheCycleAndNeverTheSameOneTwiceInARow()
    {
        var fees = TimedRoadWorkloads.BuildCyclingFees(LastCity);

        Assert.Equal(LastCity + 1, fees.Length);
        Assert.All(fees, fee => Assert.InRange(fee, SmallestFee, LargestFee));
        Assert.All(
            Enumerable.Range(0, fees.Length),
            city => Assert.Equal((city % FeeCycleLength) + SmallestFee, fees[city]));
    }

    // The budget is the straight run's cost and nothing more: every minute of it is spent making progress,
    // so a route that doubles back cannot finish inside it.
    [Fact]
    public void BudgetFor_ChainWorkload_IsExactlyWhatAStraightRunToTheLastCityCosts()
    {
        var straightRunMinutes = TimedRoadWorkloads
            .BuildStepChain(LastCity)
            .Where(road => road[1] - road[0] == UnitStepSize)
            .Sum(road => road[MinutesFieldIndex]);

        Assert.Equal(LastCity, straightRunMinutes);
        Assert.Equal(straightRunMinutes, TimedRoadWorkloads.BudgetFor(LastCity));
    }
}
