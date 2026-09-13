using DSAExperimentation.LeetCode.MinimumOneBitOperationsToMakeIntegersZero;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumOneBitOperationsToMakeIntegersZero;

// Harness only. Both strategies are MinimumOneBitOperationsToMakeIntegersZeroSolution's
// - this file just pins them to LeetCode's published examples plus the already-zero
// case, the single-flip case, and two powers/patterns far enough along the Gray code
// path that the closed form and the search would visibly disagree if either drifted.
public sealed class MinimumOneBitOperationsToMakeIntegersZeroTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 0, 0 },
            { 1, 1 },
            { 3, 2 },
            { 4, 7 },
            { 6, 4 },
            { 9, 14 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumOneBitOperationsByBreadthFirstSearch_LeetCodeExamples_ReturnsFewestFlips(
        int n, int expected) =>
        Assert.Equal(
            expected,
            MinimumOneBitOperationsToMakeIntegersZeroSolution.MinimumOneBitOperationsByBreadthFirstSearch(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumOneBitOperationsByInverseGrayCode_LeetCodeExamples_ReturnsFewestFlips(
        int n, int expected) =>
        Assert.Equal(
            expected,
            MinimumOneBitOperationsToMakeIntegersZeroSolution.MinimumOneBitOperationsByInverseGrayCode(n));
}
