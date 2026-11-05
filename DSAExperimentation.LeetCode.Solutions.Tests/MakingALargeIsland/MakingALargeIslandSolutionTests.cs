using DSAExperimentation.LeetCode.Solutions.MakingALargeIsland;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MakingALargeIsland;

// Harness only. Both strategies are MakingALargeIslandSolution's - this file just
// pins them to the two edge grids (all land,
// all water) and one grid whose flip touches the same island twice, which is what
// the labeled strategy's Set<int> dedup exists for.
public sealed partial class MakingALargeIslandSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0, 0], [0, 0]], 1 },
            { [[1, 0, 1], [0, 0, 0], [1, 0, 1]], 3 },
            { [[1, 1, 0], [1, 0, 0], [0, 0, 0]], 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestIslandByNaiveFloodFill_Cases_ReturnsLargestIslandAfterOneFlip(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MakingALargeIslandSolution.LargestIslandByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestIslandByLabeledFloodFill_Cases_ReturnsLargestIslandAfterOneFlip(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MakingALargeIslandSolution.LargestIslandByLabeledFloodFill(grid));
}
