using DSAExperimentation.LeetCode.MakingALargeIsland;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MakingALargeIsland;

// Harness only. Both strategies are MakingALargeIslandSolution's - this file just
// pins them to LeetCode's published examples plus the two edge grids (all land,
// all water) and one grid whose flip touches the same island twice, which is what
// the labeled strategy's Set<int> dedup exists for.
public sealed class MakingALargeIslandTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 0], [0, 1]], 3 },
            { [[1, 1], [1, 0]], 4 },
            { [[1, 1], [1, 1]], 4 },
            { [[0, 0], [0, 0]], 1 },
            { [[1, 0, 1], [0, 0, 0], [1, 0, 1]], 3 },
            { [[1, 1, 0], [1, 0, 0], [0, 0, 0]], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestIslandByNaiveFloodFill_LeetCodeExamples_ReturnsLargestIslandAfterOneFlip(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MakingALargeIslandSolution.LargestIslandByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestIslandByLabeledFloodFill_LeetCodeExamples_ReturnsLargestIslandAfterOneFlip(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MakingALargeIslandSolution.LargestIslandByLabeledFloodFill(grid));
}
