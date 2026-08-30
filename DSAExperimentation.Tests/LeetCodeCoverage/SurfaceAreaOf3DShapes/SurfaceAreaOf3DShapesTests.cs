namespace DSAExperimentation.Tests.LeetCodeCoverage.SurfaceAreaOf3DShapes;

// LeetCode 892. Surface Area of 3D Shapes: for each grid cell's unit-cube stack,
// sum top+bottom (2, once any cubes are stacked there) plus the exposed area on
// each of the 4 orthogonal sides - the height difference against that neighbor's
// stack, or the full stack height against the implicit height-0 ground outside the
// grid. Pure double-index array arithmetic, the same "no repo Representation/
// Operations primitive to compose" outcome TransposeMatrix/SpiralMatrixII already
// establish for fixed-shape grid index arithmetic: Grid/GridChildren model
// unordered orthogonal adjacency for graph walks (per TransposeMatrixTests' own
// stated reasoning), not a per-cell height-difference sum, so it would not be a
// genuine fit here either.
public sealed class SurfaceAreaOf3DShapesTests
{
    [Fact]
    public void SurfaceArea_SingleTallColumn_CountsAllFourSidesPlusTopAndBottom()
    {
        int[][] grid = [[2]];

        Assert.Equal(10, SurfaceArea(grid));
    }

    [Fact]
    public void SurfaceArea_TwoByTwoGrid_SubtractsSharedFaceOverlap()
    {
        int[][] grid = [[1, 2], [3, 4]];

        Assert.Equal(34, SurfaceArea(grid));
    }

    private static int SurfaceArea(int[][] grid)
    {
        var n = grid.Length;
        var area = 0;

        for (var row = 0; row < n; row++)
        {
            for (var col = 0; col < n; col++)
            {
                var height = grid[row][col];
                if (height == 0) continue;

                area += 2;
                area += ExposedSide(grid, n, height, row - 1, col);
                area += ExposedSide(grid, n, height, row + 1, col);
                area += ExposedSide(grid, n, height, row, col - 1);
                area += ExposedSide(grid, n, height, row, col + 1);
            }
        }

        return area;
    }

    private static int ExposedSide(int[][] grid, int n, int height, int row, int col)
    {
        var neighborHeight = row >= 0 && row < n && col >= 0 && col < n ? grid[row][col] : 0;
        return Math.Max(0, height - neighborHeight);
    }
}
