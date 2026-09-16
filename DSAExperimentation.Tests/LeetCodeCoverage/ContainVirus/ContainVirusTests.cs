using DSAExperimentation.LeetCode.ContainVirus;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainVirus;

// Harness only. ContainVirusSolution owns the round-by-round simulation and both
// region-discovery strategies; this file pins them to LeetCode's published
// examples.
public sealed partial class ContainVirusTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [[1, 1, 1], [1, 0, 1], [1, 1, 1]],
                4
            },
            {
                [
                    [0, 1, 0, 0, 0, 0, 0, 1],
                    [0, 1, 0, 0, 0, 0, 0, 1],
                    [0, 0, 0, 0, 0, 0, 0, 1],
                    [0, 0, 0, 0, 0, 0, 0, 0],
                ],
                10
            },
            {
                [[1]],
                0
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumWallsByNaiveRecursiveFloodFill_LeetCodeExamples_ReturnsWallsUsedToStopTheOutbreak(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ContainVirusSolution.MinimumWallsByNaiveRecursiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumWallsByDepthFirstSearchTraversal_LeetCodeExamples_ReturnsWallsUsedToStopTheOutbreak(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ContainVirusSolution.MinimumWallsByDepthFirstSearchTraversal(grid));
}
