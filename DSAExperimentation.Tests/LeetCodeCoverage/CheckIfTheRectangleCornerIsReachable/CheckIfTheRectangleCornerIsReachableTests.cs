using DSAExperimentation.LeetCode.CheckIfTheRectangleCornerIsReachable;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfTheRectangleCornerIsReachable;

// Harness only. Both strategies are
// CheckIfTheRectangleCornerIsReachableSolution's - this file just pins them to
// LeetCode's published examples, including the two-circle chain (Example 3) that only
// blocks the path once its members are unioned together, not individually.
public sealed class CheckIfTheRectangleCornerIsReachableTests
{
    public static TheoryData<int, int, int[][], bool> Examples =>
        new()
        {
            { 3, 4, new[] { new[] { 2, 1, 1 } }, true },
            { 3, 3, new[] { new[] { 1, 1, 2 } }, false },
            { 3, 3, new[] { new[] { 2, 1, 1 }, new[] { 1, 2, 1 } }, false },
            { 4, 4, new[] { new[] { 5, 5, 1 } }, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByBoundaryFloodFill_LeetCodeExamples_ReturnsWhetherACornerToCornerPathExists(
        int xCorner, int yCorner, int[][] circles, bool expected) =>
        Assert.Equal(expected, CheckIfTheRectangleCornerIsReachableSolution.IsReachableByBoundaryFloodFill(xCorner, yCorner, circles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByDisjointSet_LeetCodeExamples_ReturnsWhetherACornerToCornerPathExists(
        int xCorner, int yCorner, int[][] circles, bool expected) =>
        Assert.Equal(expected, CheckIfTheRectangleCornerIsReachableSolution.IsReachableByDisjointSet(xCorner, yCorner, circles));
}
