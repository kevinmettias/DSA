using DSAExperimentation.LeetCode.DigitOperationsToMakeTwoIntegersEqual;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DigitOperationsToMakeTwoIntegersEqual;

// Harness only: both strategies live in
// DigitOperationsToMakeTwoIntegersEqualSolution. One test method per strategy
// over one shared set of LeetCode's own examples, including the two -1 cases
// (an unreachable pair, and a target that is already prime) that the graph
// strategy has to answer without a target node existing at all.
public sealed partial class DigitOperationsToMakeTwoIntegersEqualTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 10, 12, 85 },
            { 4, 8, -1 },
            { 6, 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByBruteForceDijkstra_LeetCodeExamples_ReturnsMinimumCost(
        int startValue, int targetValue, int expected)
    {
        var actual = DigitOperationsToMakeTwoIntegersEqualSolution.MinOperationsByBruteForceDijkstra(startValue, targetValue);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByDijkstraOverDigitGraph_LeetCodeExamples_ReturnsMinimumCost(
        int startValue, int targetValue, int expected)
    {
        var actual = DigitOperationsToMakeTwoIntegersEqualSolution.MinOperationsByDijkstraOverDigitGraph(startValue, targetValue);
        Assert.Equal(expected, actual);
    }
}
