using DSAExperimentation.LeetCode.MaximumAreaRectangleWithPointConstraintsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumAreaRectangleWithPointConstraintsII;

// Harness only: both strategies are
// MaximumAreaRectangleWithPointConstraintsIISolution's. One test method per
// strategy over LeetCode's own examples, so a failure names the strategy that
// broke.
public sealed class MaximumAreaRectangleWithPointConstraintsIITests
{
    public static TheoryData<int[], int[], long> Examples =>
        new()
        {
            { [1, 1, 3, 3], [1, 3, 1, 3], 4 },
            { [1, 1, 3, 3, 2], [1, 3, 1, 3, 2], -1 },
            { [1, 1, 3, 3, 1, 3], [1, 3, 1, 3, 2, 2], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAreaByQuadrupleScan_LeetCodeExamples_ReturnsLargestUnblockedRectangleArea(
        int[] xCoord, int[] yCoord, long expected) =>
        Assert.Equal(expected, MaximumAreaRectangleWithPointConstraintsIISolution.MaxAreaByQuadrupleScan(xCoord, yCoord));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAreaBySweepSegmentTree_LeetCodeExamples_ReturnsLargestUnblockedRectangleArea(
        int[] xCoord, int[] yCoord, long expected) =>
        Assert.Equal(expected, MaximumAreaRectangleWithPointConstraintsIISolution.MaxAreaBySweepSegmentTree(xCoord, yCoord));
}
