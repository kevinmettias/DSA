using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniquePathsIII;

// LeetCode 980. Unique Paths III: this repo's generic Backtrack.Search engine
// (choose/explore/unchoose over one shared mutable state - the same shape
// SudokuSolverTests/WordSearchTests already use), closed over a 4-directional
// grid walk whose candidate set is every unvisited non-obstacle neighbor. A walk
// only counts once it lands on the "2" square having visited every non-obstacle
// square exactly once; Candidates stops offering moves the instant the walk steps
// onto "2" at all (whether or not every square has been covered yet), matching
// LeetCode's own rule that the walk must end there, not merely pass through it.
public sealed partial class UniquePathsIIITests
{
    [Fact]
    public void CountUniquePaths_OneObstacleBlocksOneOfTwoWalks_ReturnsTwo()
    {
        int[][] grid = [[1, 0, 0, 0], [0, 0, 0, 0], [0, 0, 2, -1]];
        Assert.Equal(2, CountUniquePaths(grid));
    }

    [Fact]
    public void CountUniquePaths_NoObstacles_ReturnsFour()
    {
        int[][] grid = [[1, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 2]];
        Assert.Equal(4, CountUniquePaths(grid));
    }

    [Fact]
    public void CountUniquePaths_NoWayToCoverEveryEmptySquare_ReturnsZero()
    {
        int[][] grid = [[0, 1], [2, 0]];
        Assert.Equal(0, CountUniquePaths(grid));
    }

    private static int CountUniquePaths(int[][] grid)
    {
        var (startRow, startCol) = FindValue(grid, 1);
        var state = new State(grid, startRow, startCol);
        var count = 0;

        Backtrack.Search<State, (int Row, int Col)>(
            state,
            isSolution: s => grid[s.Row][s.Col] == 2 && s.Visited == s.Total,
            candidates: s => grid[s.Row][s.Col] == 2 ? [] : s.Candidates(),
            choose: (s, next) => s.Choose(next),
            unchoose: (s, next) => s.Unchoose(next),
            onSolution: _ => count++);

        return count;
    }

    private static (int Row, int Col) FindValue(int[][] grid, int value)
    {
        for (var row = 0; row < grid.Length; row++)
        for (var col = 0; col < grid[0].Length; col++)
        {
            if (grid[row][col] == value)
            {
                return (row, col);
            }
        }

        throw new InvalidOperationException($"Grid has no cell with value {value}.");
    }

    private sealed class State
    {
        private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

        private readonly int[][] _grid;
        private readonly bool[,] _visited;
        private readonly Stack<(int Row, int Col)> _path = new();

        public State(int[][] grid, int startRow, int startCol)
        {
            _grid = grid;
            _visited = new bool[grid.Length, grid[0].Length];
            Row = startRow;
            Col = startCol;
            _visited[startRow, startCol] = true;
            Visited = 1;
            Total = CountNonObstacleCells(grid);
        }

        public int Row { get; private set; }
        public int Col { get; private set; }
        public int Visited { get; private set; }
        public int Total { get; }

        public IEnumerable<(int Row, int Col)> Candidates()
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var row = Row + dRow;
                var col = Col + dCol;

                if (row >= 0 && row < _grid.Length && col >= 0 && col < _grid[0].Length
                    && _grid[row][col] != -1 && !_visited[row, col])
                {
                    yield return (row, col);
                }
            }
        }

        public void Choose((int Row, int Col) next)
        {
            _path.Push((Row, Col));
            Row = next.Row;
            Col = next.Col;
            _visited[Row, Col] = true;
            Visited++;
        }

        public void Unchoose((int Row, int Col) next)
        {
            Visited--;
            _visited[next.Row, next.Col] = false;
            (Row, Col) = _path.Pop();
        }

        private static int CountNonObstacleCells(int[][] grid)
        {
            var total = 0;

            for (var row = 0; row < grid.Length; row++)
            for (var col = 0; col < grid[0].Length; col++)
            {
                if (grid[row][col] != -1)
                {
                    total++;
                }
            }

            return total;
        }
    }
}
