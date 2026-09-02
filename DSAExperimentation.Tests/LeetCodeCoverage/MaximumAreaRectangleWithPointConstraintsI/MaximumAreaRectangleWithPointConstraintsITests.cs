using DSAExperimentation.LeetCode.MaximumAreaRectangleWithPointConstraintsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumAreaRectangleWithPointConstraintsI;

// Harness only: both strategies are
// MaximumAreaRectangleWithPointConstraintsISolution's. One test method per
// strategy over LeetCode's own examples, so a failure names the strategy that
// broke.
public sealed class MaximumAreaRectangleWithPointConstraintsITests
{
    public static TheoryData<int[][], long> Examples =>
        new()
        {
            { [[1, 1], [1, 3], [3, 1], [3, 3]], 4 },
            { [[1, 1], [1, 3], [3, 1], [3, 3], [2, 2]], -1 },
            { [[1, 1], [1, 3], [3, 1], [3, 3], [1, 2], [3, 2]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAreaByQuadrupleScan_LeetCodeExamples_ReturnsLargestUnblockedRectangleArea(
        int[][] points, long expected) =>
        Assert.Equal(expected, MaximumAreaRectangleWithPointConstraintsISolution.MaxAreaByQuadrupleScan(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAreaByCornerLookup_LeetCodeExamples_ReturnsLargestUnblockedRectangleArea(
        int[][] points, long expected) =>
        Assert.Equal(expected, MaximumAreaRectangleWithPointConstraintsISolution.MaxAreaByCornerLookup(points));
}
