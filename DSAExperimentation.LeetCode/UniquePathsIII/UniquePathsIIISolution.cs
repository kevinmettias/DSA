using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.UniquePathsIII;

// LeetCode 980. Unique Paths III: count the 4-directionally connected walks from the
// grid's single "1" square to its single "2" square that step on every non-obstacle
// square exactly once. A walk only counts once it lands on "2" having covered
// everything - stepping onto "2" ends the walk whether or not it covered everything,
// which is LeetCode's own rule that the walk must end there rather than pass through.
//
// The two strategies differ only in who owns the choose/explore/unchoose bookkeeping:
// a hand-rolled recursion that marks visited squares by overwriting the grid in place
// and restores them on the way out, or this repo's generic Backtrack.Search engine
// closed over a State object that does the same through Choose/Unchoose.
internal static class UniquePathsIIISolution
{
    private const int StartCellMarker = 1;
    private const int EndCellMarker = 2;
    private const int ObstacleMarker = -1;

    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook answer: a plain recursive walk that marks a visited square by
    // writing ObstacleMarker over it and restores the original value as the recursion
    // unwinds, so no separate visited grid is needed. Deliberately written without
    // this repo's Backtracking primitive - it is the arm
    // CountUniquePathsByBacktrackEngine has to justify itself against. The grid is
    // mutated during the walk and left exactly as it was found.
    public static int CountUniquePathsBySpecializedRecursion(int[][] grid)
    {
        var total = CountNonObstacleCells(grid);
        var start = FindValue(grid, StartCellMarker);

        return Search(grid, start, visited: 1, total);
    }

    private static int CountNonObstacleCells(int[][] grid)
    {
        var total = 0;

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[0].Length; col++)
            {
                if (grid[row][col] != ObstacleMarker)
                {
                    total++;
                }
            }
        }

        return total;
    }

    // This repo's own choose/explore/unchoose engine: State carries the visited grid,
    // the coordinate trail Unchoose pops to restore the walk's position, and the
    // covered-square count, so every field IsSolution and Candidates read is undone
    // exactly. Candidates stops offering moves the instant the walk stands on the end
    // square, which is what makes "must end there" rather than "must pass through"
    // fall out of the engine rather than out of a special case.
    public static int CountUniquePathsByBacktrackEngine(int[][] grid)
    {
        var (startRow, startCol) = FindValue(grid, StartCellMarker);
        var state = new State(grid, startRow, startCol);
        var count = 0;

        Backtrack.Search<State, (int Row, int Col)>(
            state,
            isSolution: s => grid[s.Row][s.Col] == EndCellMarker && s.Visited == s.Total,
            candidates: s => IsAtEndCell(grid, s) ? NoMoves() : s.Candidates(),
            choose: (s, next) => s.Choose(next),
            unchoose: (s, next) => s.Unchoose(next),
            onSolution: _ => count++);

        return count;
    }

    // The walk is standing on the end square, so no move may be offered from here:
    // stepping onto "2" ends the walk whether or not it covered everything.
    private static bool IsAtEndCell(int[][] grid, State state) => grid[state.Row][state.Col] == EndCellMarker;

    // The candidate moves such a state still offers: none, which is how the engine
    // learns the walk is over rather than merely stuck.
    private static IEnumerable<(int Row, int Col)> NoMoves() => [];

    // A square the engine's walk may still step on: open ground it has not covered yet.
    private static bool IsOpenAndUnvisited(int[][] grid, bool[,] visited, int row, int col) =>
        grid[row][col] != ObstacleMarker && !visited[row, col];

    // The visited grid the walk starts from: sized to the board, with the walk's own
    // starting square already covered, since a walk covers where it stands.
    private static bool[,] StartVisitedGrid(int[][] grid, int startRow, int startCol)
    {
        var visited = new bool[grid.Length, grid[0].Length];
        visited[startRow, startCol] = true;
        return visited;
    }

    private static int Search(int[][] grid, (int Row, int Col) cell, int visited, int total)
    {
        if (grid[cell.Row][cell.Col] == EndCellMarker)
        {
            return visited == total ? 1 : 0;
        }

        return ExploreWithCellMarked(grid, cell, visited, total);
    }

    // The middle phase of one walk step: the current square is marked visited for
    // exactly as long as its neighbors are explored, and restored on the way out, so
    // the grid is mutated across the recursion and left as the walk found it.
    private static int ExploreWithCellMarked(int[][] grid, (int Row, int Col) cell, int visited, int total)
    {
        var original = MarkVisited(grid, cell);
        var count = ExploreNeighbors(grid, cell, visited, total);
        RestoreCell(grid, cell, original);

        return count;
    }

    private static int MarkVisited(int[][] grid, (int Row, int Col) cell)
    {
        var original = grid[cell.Row][cell.Col];
        grid[cell.Row][cell.Col] = ObstacleMarker;
        return original;
    }

    private static void RestoreCell(int[][] grid, (int Row, int Col) cell, int original) =>
        grid[cell.Row][cell.Col] = original;

    // Both coordinates in range is one idea, and both arms write it out per direction.
    private static bool IsInsideGrid(int row, int col, int rows, int cols) =>
        row >= 0 && row < rows && col >= 0 && col < cols;

    private static int ExploreNeighbors(int[][] grid, (int Row, int Col) cell, int visited, int total)
    {
        var count = 0;

        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (IsInsideGrid(next.Row, next.Col, grid.Length, grid[0].Length)
                && grid[next.Row][next.Col] != ObstacleMarker)
            {
                count += Search(grid, next, visited + 1, total);
            }
        }

        return count;
    }

    private static (int Row, int Col) FindValue(int[][] grid, int value)
    {
        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[0].Length; col++)
            {
                if (grid[row][col] == value)
                {
                    return (row, col);
                }
            }
        }

        throw new InvalidOperationException($"Grid has no cell with value {value}.");
    }

    // The walk's mutable state, meaningless outside LC 980's covering-walk rule, so
    // it stays beside the solution rather than becoming a shared witness.
    private sealed class State
    {
        private readonly int[][] _grid;
        private readonly bool[,] _visited;
        private readonly Stack<(int Row, int Col)> _path = new();

        public int Row { get; private set; }

        public int Col { get; private set; }
        public int Visited { get; private set; }
        public int Total { get; }
        public State(int[][] grid, int startRow, int startCol)
        {
            _grid = grid;
            _visited = StartVisitedGrid(grid, startRow, startCol);
            Row = startRow;
            Col = startCol;
            Visited = 1;
            Total = CountNonObstacleCells(grid);
        }

        public IEnumerable<(int Row, int Col)> Candidates()
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var row = Row + dRow;
                var col = Col + dCol;

                if (IsInsideGrid(row, col, _grid.Length, _grid[0].Length)
                    && IsOpenAndUnvisited(_grid, _visited, row, col))
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
