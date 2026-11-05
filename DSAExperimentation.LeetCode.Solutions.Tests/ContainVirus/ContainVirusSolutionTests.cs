using DSAExperimentation.LeetCode.Solutions.ContainVirus;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ContainVirus;

// Harness only. ContainVirusSolution owns the round-by-round simulation and both
// region-discovery strategies; this file pins them to a grid whose only cell is
// infected, so no cell is ever threatened and no wall is built. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class ContainVirusSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            {
                [[1]],
                0
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumWallsByNaiveRecursiveFloodFill_Cases_ReturnsWallsUsedToStopTheOutbreak(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ContainVirusSolution.MinimumWallsByNaiveRecursiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumWallsByDepthFirstSearchTraversal_Cases_ReturnsWallsUsedToStopTheOutbreak(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ContainVirusSolution.MinimumWallsByDepthFirstSearchTraversal(grid));
}
