namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathInBinaryMatrix;

// LeetCode 1091. Shortest Path in Binary Matrix: single-source BFS from the top-left
// cell, the same Queue<TElement>-as-frontier shape RottingOrangesTests/
// ZeroOneMatrixTests already use for their own grid BFS sweeps - just walking all 8
// king-move directions instead of 4, since diagonal steps are allowed here.
// Graph/Grids' own GridChildren is hardcoded to the 4 orthogonal directions (no
// diagonal support), so this composes the repo's Queue<TElement> directly rather
// than going through that topology - the same choice RottingOranges/ZeroOneMatrix
// already made for the 4-directional case.
public sealed partial class ShortestPathInBinaryMatrixTests
{
    [Fact]
    public void ShortestPathBinaryMatrix_ClassicExample_ReturnsDiagonalPathLength()
    {
        int[][] grid = [[0, 1], [1, 0]];

        Assert.Equal(2, ShortestPathBinaryMatrix(grid));
    }

    [Fact]
    public void ShortestPathBinaryMatrix_SecondExample_ReturnsShortestPathLength()
    {
        int[][] grid = [[0, 0, 0], [1, 1, 0], [1, 1, 0]];

        Assert.Equal(4, ShortestPathBinaryMatrix(grid));
    }

    [Fact]
    public void ShortestPathBinaryMatrix_NoClearPath_ReturnsNegativeOne()
    {
        int[][] grid = [[1, 0, 0], [1, 1, 0], [1, 1, 0]];

        Assert.Equal(-1, ShortestPathBinaryMatrix(grid));
    }

    private static readonly (int DRow, int DCol)[] Directions =
    [
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1),
    ];

    private static int ShortestPathBinaryMatrix(int[][] grid)
    {
        var n = grid.Length;

        if (grid[0][0] != 0 || grid[n - 1][n - 1] != 0)
        {
            return -1;
        }

        var distance = new int[n][];
        for (var r = 0; r < n; r++)
        {
            distance[r] = new int[n];
            Array.Fill(distance[r], -1);
        }

        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();
        distance[0][0] = 1;
        frontier.Enqueue((0, 0));

        while (frontier.TryDequeue(out var cell))
        {
            if (cell.Row == n - 1 && cell.Col == n - 1)
            {
                return distance[cell.Row][cell.Col];
            }

            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = cell.Row + dRow;
                var nextCol = cell.Col + dCol;

                if (nextRow < 0 || nextRow >= n || nextCol < 0 || nextCol >= n
                    || grid[nextRow][nextCol] != 0 || distance[nextRow][nextCol] != -1)
                {
                    continue;
                }

                distance[nextRow][nextCol] = distance[cell.Row][cell.Col] + 1;
                frontier.Enqueue((nextRow, nextCol));
            }
        }

        return -1;
    }
}
