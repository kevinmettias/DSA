using DSAExperimentation.LeetCode.UniquePathsIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniquePathsIII;

// Harness only. Both counting strategies are UniquePathsIIISolution's - this file
// just pins them to LeetCode's published examples, including the grid where no walk
// can cover every empty square, plus a two-cell grid where the walk ends on its very
// first step.
public sealed partial class UniquePathsIIITests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 0, 0, 0], [0, 0, 0, 0], [0, 0, 2, -1]], 2 },
            { [[1, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 2]], 4 },
            { [[0, 1], [2, 0]], 0 },
            { [[1, 2]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountUniquePathsBySpecializedRecursion_LeetCodeExamples_ReturnsCoveringWalkCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, UniquePathsIIISolution.CountUniquePathsBySpecializedRecursion(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountUniquePathsByBacktrackEngine_LeetCodeExamples_ReturnsCoveringWalkCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, UniquePathsIIISolution.CountUniquePathsByBacktrackEngine(grid));
}
