namespace DSAExperimentation.Tests.LeetCodeCoverage.ProjectionAreaOf3DShapes;

// LeetCode 883. Projection Area of 3D Shapes: sum the three orthographic
// projections of a height grid - top (count of occupied cells), front (each
// row's tallest stack), side (each column's tallest stack). A direct
// O(rows*cols) grid reduction; no repo Representation/Operations primitive
// applies (the same "nothing to compose" precedent Spiral Matrix/Spiral
// Matrix II already establish) since there is nothing here beyond a plain
// int[][] and Math.Max.
public sealed partial class ProjectionAreaOf3DShapesTests
{
    [Fact]
    public void ProjectionArea_ClassicExample_ReturnsSummedAreas()
    {
        int[][] grid = [[1, 2], [3, 4]];

        Assert.Equal(17, ProjectionArea(grid));
    }

    [Fact]
    public void ProjectionArea_AllZeroGrid_ReturnsZero()
    {
        int[][] grid = [[0, 0], [0, 0]];

        Assert.Equal(0, ProjectionArea(grid));
    }

    private static int ProjectionArea(int[][] grid)
    {
        var (top, front) = ComputeTopAndFront(grid);
        var side = ComputeSide(grid);

        return top + front + side;
    }

    private static (int Top, int Front) ComputeTopAndFront(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var top = 0;
        var front = 0;

        for (var r = 0; r < rows; r++)
        {
            var rowMax = 0;
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] > 0)
                {
                    top++;
                }

                rowMax = Math.Max(rowMax, grid[r][c]);
            }

            front += rowMax;
        }

        return (top, front);
    }

    private static int ComputeSide(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var side = 0;

        for (var c = 0; c < cols; c++)
        {
            var colMax = 0;
            for (var r = 0; r < rows; r++)
            {
                colMax = Math.Max(colMax, grid[r][c]);
            }

            side += colMax;
        }

        return side;
    }
}
