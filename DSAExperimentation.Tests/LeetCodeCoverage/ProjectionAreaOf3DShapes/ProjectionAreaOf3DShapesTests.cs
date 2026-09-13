using DSAExperimentation.LeetCode.ProjectionAreaOf3DShapes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ProjectionAreaOf3DShapes;

// Harness only. All three grid reductions - the three-pass baseline, the two-pass
// row/column split the pre-migration test carried, and the fused single pass - are
// ProjectionAreaOf3DShapesSolution's; this file just pins them to LeetCode's
// published examples.
public sealed class ProjectionAreaOf3DShapesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: top 4, front 2 + 4, side 3 + 4.
            { [[1, 2], [3, 4]], 17 },

            // LeetCode example 2: a single stack of height 2 projects 1 + 2 + 2.
            { [[2]], 5 },

            // LeetCode example 3: the two occupied cells lie on opposite diagonals.
            { [[1, 0], [0, 2]], 8 },

            // Empty space everywhere: no projection has any area.
            { [[0, 0], [0, 0]], 0 },

            // A hole in the middle: the top view loses a cell the two side-on views
            // still see over, so the three projections disagree about that column.
            { [[1, 1, 1], [1, 0, 1], [1, 1, 1]], 14 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProjectionAreaByThreeSeparatePasses_LeetCodeExamples_ReturnsSummedProjectionAreas(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ProjectionAreaOf3DShapesSolution.ProjectionAreaByThreeSeparatePasses(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProjectionAreaByRowAndColumnPasses_LeetCodeExamples_ReturnsSummedProjectionAreas(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ProjectionAreaOf3DShapesSolution.ProjectionAreaByRowAndColumnPasses(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProjectionAreaBySingleCombinedPass_LeetCodeExamples_ReturnsSummedProjectionAreas(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ProjectionAreaOf3DShapesSolution.ProjectionAreaBySingleCombinedPass(grid));
}
