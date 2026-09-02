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

        var distance = BuildDistanceGrid(n);
        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();
        distance[0][0] = 1;
        frontier.Enqueue((0, 0));

        var state = new PathBfsState(grid, distance, frontier);

        return RunBfs(state);
    }

    private static int[][] BuildDistanceGrid(int n)
    {
        var distance = new int[n][];
        for (var r = 0; r < n; r++)
        {
            distance[r] = new int[n];
            Array.Fill(distance[r], -1);
        }

        return distance;
    }

    // Runs the BFS to completion, returning the distance to the bottom-right cell
    // the moment it's dequeued, or -1 if the frontier empties out first.
    private static int RunBfs(PathBfsState state)
    {
        while (state.Frontier.TryDequeue(out var cell))
        {
            if (cell.Row == state.N - 1 && cell.Col == state.N - 1)
            {
                return state.Distance[cell.Row][cell.Col];
            }

            foreach (var direction in Directions)
            {
                EnqueueNeighborIfReachable(state, cell, direction);
            }
        }

        return -1;
    }

    // One BFS relaxation step: enqueues the neighbor across `direction` from `cell`
    // if it's open ground and not already reached.
    private static void EnqueueNeighborIfReachable(PathBfsState state, (int Row, int Col) cell, (int DRow, int DCol) direction)
    {
        var nextRow = cell.Row + direction.DRow;
        var nextCol = cell.Col + direction.DCol;

        if (nextRow < 0 || nextRow >= state.N || nextCol < 0 || nextCol >= state.N
            || state.Grid[nextRow][nextCol] != 0 || state.Distance[nextRow][nextCol] != -1)
        {
            return;
        }

        state.Distance[nextRow][nextCol] = state.Distance[cell.Row][cell.Col] + 1;
        state.Frontier.Enqueue((nextRow, nextCol));
    }

    // The grid/distance-map/frontier the BFS reads and writes, bundled together so
    // EnqueueNeighborIfReachable doesn't need one parameter per collection.
    private sealed class PathBfsState(
        int[][] grid, int[][] distance, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier)
    {
        public readonly int[][] Grid = grid;
        public readonly int[][] Distance = distance;
        public readonly DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> Frontier = frontier;
        public readonly int N = grid.Length;
    }
}
