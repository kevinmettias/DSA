using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for GasStationWorkloads (ARCHITECTURE 17.7). The reading depends on the circuit
// having exactly one valid start, the last station - LC 134 guarantees a unique answer, and the last
// station is what forces the brute-force baseline through a failing lap from every earlier start -
// and on every amount staying inside LC 134's 0..10^4. Both are checked by simulating every start
// directly, independently of the construction's running-sum argument.
public sealed partial class GasStationWorkloadsTests
{
    private const int Seed = 134;
    private const int SmallLength = 200;
    private const int LargeLength = 3_000;

    // LC 134's amounts.
    private const int LowestAmount = 0;
    private const int HighestAmount = 10_000;

    [Theory]
    [InlineData(SmallLength)]
    [InlineData(LargeLength)]
    public void Build_EveryStart_CompletesTheCircuitOnlyFromTheLastStation(int length)
    {
        var (gas, cost) = GasStationWorkloads.Build(length, Seed);

        var validStarts = Enumerable.Range(0, length).Where(start => IsCompletableFrom(gas, cost, start));
        int[] lastStationOnly = [length - 1];

        Assert.Equal(lastStationOnly, validStarts);
    }

    [Theory]
    [InlineData(SmallLength)]
    [InlineData(LargeLength)]
    public void Build_EveryAmount_StaysInsideLeetCodesRange(int length)
    {
        var (gas, cost) = GasStationWorkloads.Build(length, Seed);

        Assert.All(gas.Concat(cost), amount => Assert.InRange(amount, LowestAmount, HighestAmount));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameStations()
    {
        var first = GasStationWorkloads.Build(SmallLength, Seed);
        var second = GasStationWorkloads.Build(SmallLength, Seed);

        Assert.Equal(first.Gas, second.Gas);
        Assert.Equal(first.Cost, second.Cost);
    }

    // One full lap from start, refusing the moment the tank would run dry.
    private static bool IsCompletableFrom(int[] gas, int[] cost, int start)
    {
        var tank = 0;

        for (var step = 0; step < gas.Length; step++)
        {
            var station = (start + step) % gas.Length;
            tank += gas[station] - cost[station];

            if (tank < 0)
            {
                return false;
            }
        }

        return true;
    }
}
