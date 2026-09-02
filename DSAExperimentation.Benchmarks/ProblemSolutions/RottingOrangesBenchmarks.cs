using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Rotting Oranges (LC 994): an independent BFS per fresh orange (a freshly
// allocated visited grid and BCL Queue re-walked outward from each fresh cell
// until it hits a rotten one, taking the max over all of them) vs. one shared
// multi-source BFS using this repo's own Queue<TElement>, seeded with every
// already-rotten orange at once - O(rows*cols) total instead of O(rows*cols) work
// times one BFS per fresh starting cell.
[MemoryDiagnoser]
public class RottingOrangesBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // random.Next(0, CellStateRoll) weights the empty/fresh/rotten cell distribution
    // generated below.
    private const int CellStateRoll = 10;

    private const int RottenOrangeState = 2;

    [Params(10, 25)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, CellStateRoll) switch
            {
                0 => RottenOrangeState,
                1 => 0,
                _ => 1,
            }).ToArray())
            .ToArray();
        _grid[0][0] = RottenOrangeState;
    }

    [Benchmark(Baseline = true)]
    public int PerCellBfs()
    {
        var maxMinutes = 0;

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (!TryAccumulateFreshCellMinutes(r, c, ref maxMinutes))
                {
                    return -1;
                }
            }
        }

        return maxMinutes;
    }

    // Handles one grid cell for PerCellBfs: skips non-fresh cells, otherwise folds
    // its nearest-rotten distance into maxMinutes. Returns false to signal that no
    // rotten orange can reach this cell, so the caller must abort with -1.
    private bool TryAccumulateFreshCellMinutes(int row, int col, ref int maxMinutes)
    {
        if (_grid[row][col] != 1)
        {
            return true;
        }

        var minutes = NearestRottenDistance(row, col);

        if (minutes < 0)
        {
            return false;
        }

        maxMinutes = Math.Max(maxMinutes, minutes);
        return true;
    }

    private int NearestRottenDistance(int startRow, int startCol)
    {
        var visited = new bool[Size, Size];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col, int Minutes)>();
        visited[startRow, startCol] = true;
        queue.Enqueue((startRow, startCol, 0));

        var state = new SingleSourceBfsState(visited, queue);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (TryExpandCell(current, state, out var rottenMinutes))
            {
                return rottenMinutes;
            }
        }

        return -1;
    }

    // Visits the four neighbors of the dequeued cell: if one is rotten, reports the
    // distance to it; otherwise enqueues every unvisited fresh neighbor for later
    // expansion.
    private bool TryExpandCell((int Row, int Col, int Minutes) current, SingleSourceBfsState state, out int rottenMinutes)
    {
        foreach (var direction in Directions)
        {
            if (TryVisitNeighbor(current, direction, state, out var foundMinutes))
            {
                rottenMinutes = foundMinutes;
                return true;
            }
        }

        rottenMinutes = -1;
        return false;
    }

    // Checks a single neighbor in the given direction: reports the distance if it's
    // rotten, enqueues it if it's an unvisited fresh cell, otherwise does nothing.
    // The self-contained per-neighbor step of the BFS above.
    private bool TryVisitNeighbor(
        (int Row, int Col, int Minutes) current,
        (int DRow, int DCol) direction,
        SingleSourceBfsState state,
        out int rottenMinutes)
    {
        var nextRow = current.Row + direction.DRow;
        var nextCol = current.Col + direction.DCol;
        rottenMinutes = -1;

        if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || state.Visited[nextRow, nextCol])
        {
            return false;
        }

        if (_grid[nextRow][nextCol] == RottenOrangeState)
        {
            rottenMinutes = current.Minutes + 1;
            return true;
        }

        if (_grid[nextRow][nextCol] == 1)
        {
            state.Visited[nextRow, nextCol] = true;
            state.Queue.Enqueue((nextRow, nextCol, current.Minutes + 1));
        }

        return false;
    }

    // The visited grid and frontier queue a single-source BFS expands into -
    // bundled so TryVisitNeighbor stays within the parameter-count limit.
    private readonly record struct SingleSourceBfsState(
        bool[,] Visited,
        System.Collections.Generic.Queue<(int Row, int Col, int Minutes)> Queue);

    [Benchmark]
    public int MultiSourceBfs()
    {
        var (grid, progress) = InitializeRotGrid();
        Drain(grid, ref progress);
        return progress.FreshCount == 0 ? progress.MinutesElapsed : -1;
    }

    // Allocates the per-cell arrays, then scans the grid once to seed the BFS
    // frontier with every already-rotten cell and count the fresh ones.
    private (RotGrid Grid, BfsProgress Progress) InitializeRotGrid()
    {
        var minutesToRot = new int[Size][];
        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();
        var visited = new bool[Size, Size];
        var progress = new BfsProgress();

        ScanGridForSeeds(minutesToRot, visited, frontier, ref progress);

        return (new RotGrid(visited, minutesToRot, frontier), progress);
    }

    // Allocates each row of minutesToRot, then enqueues every already-rotten cell
    // as a BFS seed and counts the fresh ones. The self-contained scan step of the
    // initialization above.
    private void ScanGridForSeeds(
        int[][] minutesToRot,
        bool[,] visited,
        DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier,
        ref BfsProgress progress)
    {
        for (var r = 0; r < Size; r++)
        {
            minutesToRot[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                if (_grid[r][c] == RottenOrangeState)
                {
                    frontier.Enqueue((r, c));
                    visited[r, c] = true;
                }
                else if (_grid[r][c] == 1)
                {
                    progress.FreshCount++;
                }
            }
        }
    }

    // Drains the frontier, expanding one rotten cell at a time until none remain.
    private void Drain(RotGrid grid, ref BfsProgress progress)
    {
        while (grid.Frontier.TryDequeue(out var cell))
        {
            ExpandRottenCell(cell, grid, ref progress);
        }
    }

    // Visits the four neighbors of a just-dequeued rotten/rotting cell: marks each
    // reachable fresh neighbor rotten, records its minute, and enqueues it. The
    // self-contained per-cell step of the multi-source BFS above.
    private void ExpandRottenCell((int Row, int Col) cell, RotGrid grid, ref BfsProgress progress)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = cell.Row + dRow;
            var nextCol = cell.Col + dCol;

            if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size
                || grid.Visited[nextRow, nextCol] || _grid[nextRow][nextCol] != 1)
            {
                continue;
            }

            grid.Visited[nextRow, nextCol] = true;
            grid.MinutesToRot[nextRow][nextCol] = grid.MinutesToRot[cell.Row][cell.Col] + 1;
            progress.FreshCount--;
            progress.MinutesElapsed = Math.Max(progress.MinutesElapsed, grid.MinutesToRot[nextRow][nextCol]);
            grid.Frontier.Enqueue((nextRow, nextCol));
        }
    }

    // The three shared, in-place-mutated structures the multi-source BFS expands
    // into - bundled so ExpandRottenCell stays within the parameter-count limit.
    private readonly record struct RotGrid(
        bool[,] Visited,
        int[][] MinutesToRot,
        DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> Frontier);

    // The two running totals ExpandRottenCell updates on every reachable neighbor.
    private struct BfsProgress
    {
        public int FreshCount;
        public int MinutesElapsed;
    }
}
