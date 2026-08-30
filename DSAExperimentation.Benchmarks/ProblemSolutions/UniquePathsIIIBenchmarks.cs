using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Unique Paths III (LC 980): a hand-specialized recursive walk (mutating the grid
// in place to mark visited cells, restoring on backtrack) vs. this repo's generic
// Backtrack.Search engine closed over the same choose/explore/unchoose shape. The
// search tree's size is far more sensitive to obstacle layout than to raw grid
// dimensions - a Hamiltonian-path count can blow up combinatorially even on a
// small board - so, the same reasoning NQueensBenchmarks/SudokuSolverBenchmarks
// already give for fixing their own board/size rather than [Params]-ing it, this
// fixes one small grid and isolates the constant-factor cost of Backtrack.Search's
// generic delegate dispatch from an equivalent purpose-built recursion.
[MemoryDiagnoser]
public class UniquePathsIIIBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup() => _grid = [[1, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 2]];

    [Benchmark(Baseline = true)]
    public int SpecializedRecursive() => CountSpecialized(_grid);

    [Benchmark]
    public int BacktrackEngine() => CountWithBacktrackEngine(_grid);

    private static int CountSpecialized(int[][] grid)
    {
        var total = CountNonObstacleCells(grid);
        var (startRow, startCol) = FindValue(grid, 1);
        var count = 0;

        void Search(int row, int col, int visited)
        {
            if (grid[row][col] == 2)
            {
                if (visited == total)
                {
                    count++;
                }

                return;
            }

            var original = grid[row][col];
            grid[row][col] = -1;

            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = row + dRow;
                var nextCol = col + dCol;

                if (nextRow >= 0 && nextRow < grid.Length && nextCol >= 0 && nextCol < grid[0].Length
                    && grid[nextRow][nextCol] != -1)
                {
                    Search(nextRow, nextCol, visited + 1);
                }
            }

            grid[row][col] = original;
        }

        Search(startRow, startCol, 1);
        return count;
    }

    private static int CountWithBacktrackEngine(int[][] grid)
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

    private sealed class State
    {
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
    }
}
