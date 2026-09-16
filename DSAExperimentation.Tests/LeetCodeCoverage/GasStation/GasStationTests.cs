using DSAExperimentation.LeetCode.GasStation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GasStation;

// Harness only: both strategies live in GasStationSolution and are asserted
// against the same examples - the greedy debt-reset scan this file's
// original private helper computed, and the O(n^2) brute-force simulation
// that used to be untested benchmark scaffolding.
public sealed class GasStationTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], [3, 4, 5, 1, 2], 3 },
            { [2, 3, 4], [3, 4, 3], -1 },
            { [5], [4], 0 },
            { [3, 1, 1], [1, 2, 2], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanCompleteCircuitByGreedyDebtReset_LeetCodeExamples_ReturnsStartIndex(
        int[] gas, int[] cost, int expected)
    {
        var actual = GasStationSolution.CanCompleteCircuitByGreedyDebtReset(gas, cost);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanCompleteCircuitByBruteForceSimulation_LeetCodeExamples_ReturnsStartIndex(
        int[] gas, int[] cost, int expected)
    {
        var actual = GasStationSolution.CanCompleteCircuitByBruteForceSimulation(gas, cost);

        Assert.Equal(expected, actual);
    }
}
