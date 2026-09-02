using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Contain Virus (LC 749): the same round-by-round simulation (find regions, rank by
// threat, quarantine the worst, spread the rest) run over two region-discovery
// strategies - a hand-specialized recursive flood fill (the textbook approach,
// matching MaxAreaOfIslandBenchmarks' own NaiveRecursiveFloodFill) vs. this repo's
// own DepthFirstSearch.Traverse, the same border-flood-fill primitive
// MaxAreaOfIslandTests/ContainVirusTests already use for LC 695/749. Regions are
// rediscovered fresh every round since quarantining and spreading both mutate the
// grid, so this is genuinely repeated work, not a one-off setup cost.
[MemoryDiagnoser]
public class ContainVirusBenchmarks
{
    private const int RandomSeed = 749; // LeetCode problem number

    private const double InfectionSeedProbability = 0.15;

    [Params(10, 25)]
    public int Side;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _grid[r] = new int[Side];

            for (var c = 0; c < Side; c++)
            {
                _grid[r][c] = random.NextDouble() < InfectionSeedProbability ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill() => MinimumWalls(CloneGrid(), CollectRegionRecursive);

    [Benchmark]
    public int DepthFirstSearchTraversal() => MinimumWalls(CloneGrid(), CollectRegionViaTraversal);

    private static int MinimumWalls(int[][] grid, CollectRegion collectRegion)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var totalWalls = 0;

        while (true)
        {
            var (done, wallsAdded) = RunQuarantineRound(grid, rows, cols, collectRegion);

            if (done)
            {
                return totalWalls;
            }

            totalWalls += wallsAdded;
        }
    }

    private static (bool Done, int WallsAdded) RunQuarantineRound(
        int[][] grid, int rows, int cols, CollectRegion collectRegion)
    {
        var regions = FindRegions(grid, rows, cols, collectRegion);
        var mostThreatening = regions.MaxBy(region => region.Threatened.Count);

        if (mostThreatening is null || mostThreatening.Threatened.Count == 0)
        {
            return (true, 0);
        }

        QuarantineRegion(grid, mostThreatening);
        SpreadRemainingRegions(grid, regions, mostThreatening);

        return (false, mostThreatening.WallsNeeded);
    }

    private static void QuarantineRegion(int[][] grid, Region region)
    {
        foreach (var (row, col) in region.Cells)
        {
            grid[row][col] = -1;
        }
    }

    private static void SpreadRemainingRegions(int[][] grid, List<Region> regions, Region quarantined)
    {
        foreach (var region in regions)
        {
            if (region == quarantined)
            {
                continue;
            }

            foreach (var (row, col) in region.Threatened)
            {
                grid[row][col] = 1;
            }
        }
    }

    private delegate List<(int Row, int Col)> CollectRegion(int[][] grid, int rows, int cols, (int Row, int Col) start);

    private sealed record GridView(int[][] Grid, int Rows, int Cols);

    private static List<Region> FindRegions(int[][] grid, int rows, int cols, CollectRegion collectRegion)
    {
        var visited = new HashSet<(int Row, int Col)>();
        var regions = new List<Region>();
        var view = new GridView(grid, rows, cols);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 1 || !visited.Add((r, c)))
                {
                    continue;
                }

                var region = DiscoverRegion(view, collectRegion, (r, c), visited);
                regions.Add(region);
            }
        }

        return regions;
    }

    private static Region DiscoverRegion(
        GridView view, CollectRegion collectRegion, (int Row, int Col) start, HashSet<(int Row, int Col)> visited)
    {
        var cells = collectRegion(view.Grid, view.Rows, view.Cols, start);

        foreach (var cell in cells)
        {
            visited.Add(cell);
        }

        return BuildRegion(view.Grid, view.Rows, view.Cols, cells);
    }

    private static Region BuildRegion(int[][] grid, int rows, int cols, List<(int Row, int Col)> cells)
    {
        var threatened = new HashSet<(int Row, int Col)>();
        var wallsNeeded = 0;

        foreach (var cell in cells)
        {
            foreach (var neighbor in FourNeighbors(cell, rows, cols))
            {
                if (grid[neighbor.Row][neighbor.Col] == 0)
                {
                    threatened.Add(neighbor);
                    wallsNeeded++;
                }
            }
        }

        return new Region(cells, threatened, wallsNeeded);
    }

    private sealed record FloodState(List<(int Row, int Col)> Cells, HashSet<(int Row, int Col)> Visited);

    private static List<(int Row, int Col)> CollectRegionRecursive(
        int[][] grid, int rows, int cols, (int Row, int Col) start)
    {
        var view = new GridView(grid, rows, cols);
        var state = new FloodState([], []);

        Flood(start, view, state);

        return state.Cells;
    }

    private static void Flood((int Row, int Col) cell, GridView view, FloodState state)
    {
        if (cell.Row < 0 || cell.Row >= view.Rows || cell.Col < 0 || cell.Col >= view.Cols)
        {
            return;
        }

        if (view.Grid[cell.Row][cell.Col] != 1 || !state.Visited.Add(cell))
        {
            return;
        }

        state.Cells.Add(cell);
        Flood((cell.Row - 1, cell.Col), view, state);
        Flood((cell.Row + 1, cell.Col), view, state);
        Flood((cell.Row, cell.Col - 1), view, state);
        Flood((cell.Row, cell.Col + 1), view, state);
    }

    private static List<(int Row, int Col)> CollectRegionViaTraversal(
        int[][] grid, int rows, int cols, (int Row, int Col) start)
        => DepthFirstSearch.Traverse(start, cell => InfectedNeighbors(grid, rows, cols, cell));

    private static IEnumerable<(int Row, int Col)> InfectedNeighbors(
        int[][] grid, int rows, int cols, (int Row, int Col) cell)
    {
        foreach (var neighbor in FourNeighbors(cell, rows, cols))
        {
            if (grid[neighbor.Row][neighbor.Col] == 1)
            {
                yield return neighbor;
            }
        }
    }

    private static IEnumerable<(int Row, int Col)> FourNeighbors((int Row, int Col) cell, int rows, int cols)
    {
        (int Row, int Col)[] candidates =
        [
            (cell.Row - 1, cell.Col), (cell.Row + 1, cell.Col),
            (cell.Row, cell.Col - 1), (cell.Row, cell.Col + 1),
        ];

        foreach (var candidate in candidates)
        {
            if (candidate.Row >= 0 && candidate.Row < rows && candidate.Col >= 0 && candidate.Col < cols)
            {
                yield return candidate;
            }
        }
    }

    private sealed class Region(List<(int Row, int Col)> cells, HashSet<(int Row, int Col)> threatened, int wallsNeeded)
    {
        public List<(int Row, int Col)> Cells { get; } = cells;

        public HashSet<(int Row, int Col)> Threatened { get; } = threatened;

        public int WallsNeeded { get; } = wallsNeeded;
    }

    private int[][] CloneGrid()
    {
        var clone = new int[_grid.Length][];

        for (var r = 0; r < _grid.Length; r++)
        {
            clone[r] = (int[])_grid[r].Clone();
        }

        return clone;
    }
}
