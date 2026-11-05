using DSAExperimentation.LeetCode.Solutions.GasStation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GasStation;

// Harness only: both strategies live in GasStationSolution and are asserted
// against the same cases - the greedy debt-reset scan this file's
// original private helper computed, and the O(n^2) brute-force simulation
// that used to be untested benchmark scaffolding.
public sealed partial class GasStationSolutionTests
{
    public static TheoryData<int[], int[], int> Cases =>
        new()
        {
            { [5], [4], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanCompleteCircuitByGreedyDebtReset_Cases_ReturnsStartIndex(
        int[] gas, int[] cost, int expected)
    {
        var actual = GasStationSolution.CanCompleteCircuitByGreedyDebtReset(gas, cost);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanCompleteCircuitByBruteForceSimulation_Cases_ReturnsStartIndex(
        int[] gas, int[] cost, int expected)
    {
        var actual = GasStationSolution.CanCompleteCircuitByBruteForceSimulation(gas, cost);

        Assert.Equal(expected, actual);
    }
}
