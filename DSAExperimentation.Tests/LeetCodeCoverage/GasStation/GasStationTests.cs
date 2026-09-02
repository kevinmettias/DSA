namespace DSAExperimentation.Tests.LeetCodeCoverage.GasStation;

public sealed class GasStationTests
{
    [Theory]
    [InlineData(new[] { 1,2,3,4,5 }, new[] { 3,4,5,1,2 }, 3)]
    [InlineData(new[] { 2,3,4 }, new[] { 3,4,3 }, -1)]
    public void CanCompleteCircuit_GreedyDebtReset_ReturnsStartIndex(int[] gas, int[] cost, int expected) { var actual = CanCompleteCircuit(gas, cost); Assert.Equal(expected, actual); }

    private static int CanCompleteCircuit(int[] gas, int[] cost)
    {
        var total = 0; var tank = 0; var start = 0;
        for (var i = 0; i < gas.Length; i++) { var delta = gas[i] - cost[i]; total += delta; tank += delta; if (tank < 0) { start = i + 1; tank = 0; } }
        return total < 0 ? -1 : start;
    }
}
