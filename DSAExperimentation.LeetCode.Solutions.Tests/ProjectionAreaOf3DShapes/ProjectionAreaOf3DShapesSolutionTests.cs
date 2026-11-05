using DSAExperimentation.LeetCode.Solutions.ProjectionAreaOf3DShapes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ProjectionAreaOf3DShapes;

// Harness only. All three grid reductions - the three-pass baseline, the two-pass
// row/column split the pre-migration test carried, and the fused single pass - are
// ProjectionAreaOf3DShapesSolution's; this file just pins them to an all-zero grid.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class ProjectionAreaOf3DShapesSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // Empty space everywhere: no projection has any area.
            { [[0, 0], [0, 0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ProjectionAreaByThreeSeparatePasses_Cases_ReturnsSummedProjectionAreas(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ProjectionAreaOf3DShapesSolution.ProjectionAreaByThreeSeparatePasses(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ProjectionAreaByRowAndColumnPasses_Cases_ReturnsSummedProjectionAreas(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ProjectionAreaOf3DShapesSolution.ProjectionAreaByRowAndColumnPasses(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ProjectionAreaBySingleCombinedPass_Cases_ReturnsSummedProjectionAreas(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ProjectionAreaOf3DShapesSolution.ProjectionAreaBySingleCombinedPass(grid));
}
