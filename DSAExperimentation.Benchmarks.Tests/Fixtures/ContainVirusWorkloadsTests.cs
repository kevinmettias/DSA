using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ContainVirusWorkloads (ARCHITECTURE 17.7). LC 749 promises that each round exactly one
// region threatens the most uninfected cells; a grid that breaks it admits two quarantine orders, so its answer
// is no longer fixed by the input alone. The seeded grid at every side ContainVirusBenchmarks runs is replayed
// here round by round - wall off the leading region, let every other region spread - without either arm, and
// no round may tie. Seed 749, the one the benchmark used before, ties a round at both sides.
public sealed partial class ContainVirusWorkloadsTests
{
    private const int BenchmarkSeed = 752;
    private const int Healthy = 0;
    private const int Infected = 1;
    private const int Quarantined = 2;

    private static readonly (int Row, int Col)[] Steps = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    public static TheoryData<int> BenchmarkSides => [10, 25];

    [Theory]
    [MemberData(nameof(BenchmarkSides))]
    public void BuildGrid_BenchmarkSeed_NeverTiesForTheMostThreateningRegion(int side) =>
        Assert.Empty(TiedRounds(ContainVirusWorkloads.BuildGrid(side, BenchmarkSeed)));

    // Plays LC 749 out on the grid and returns every round whose most-threatening region was not unique.
    private static List<int> TiedRounds(int[][] grid)
    {
        var tiedRounds = new List<int>();
        var threatening = ThreateningRegions(grid);
        for (var round = 1; threatening.Count > 0; round++)
        {
            var most = threatening.Max(region => region.Threatened.Count);
            var leaders = threatening.Where(region => region.Threatened.Count == most).ToList();
            if (leaders.Count > 1)
            {
                tiedRounds.Add(round);
            }

            AdvanceOneRound(grid, threatening, leaders[0]);
            threatening = ThreateningRegions(grid);
        }

        return tiedRounds;
    }

    // Walls the leader off, then lets every other region infect each cell it threatens.
    private static void AdvanceOneRound(int[][] grid, List<Region> threatening, Region leader)
    {
        foreach (var (row, col) in leader.Cells)
        {
            grid[row][col] = Quarantined;
        }

        foreach (var region in threatening.Where(region => !ReferenceEquals(region, leader)))
        {
            foreach (var (row, col) in region.Threatened)
            {
                grid[row][col] = Infected;
            }
        }
    }

    // Every connected block of infected cells with at least one healthy neighbour, in row-major order of discovery.
    private static List<Region> ThreateningRegions(int[][] grid)
    {
        var seen = new HashSet<(int Row, int Col)>();
        var regions = new List<Region>();
        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] == Infected && seen.Add((row, col)))
                {
                    regions.Add(ExploreRegion(grid, (row, col), seen));
                }
            }
        }

        return regions.Where(region => region.Threatened.Count > 0).ToList();
    }

    // The infected block containing start, and the distinct healthy cells it borders.
    private static Region ExploreRegion(int[][] grid, (int Row, int Col) start, HashSet<(int Row, int Col)> seen)
    {
        var region = new Region([], []);
        var pending = new List<(int Row, int Col)> { start };
        while (pending.Count > 0)
        {
            var cell = pending[^1];
            pending.RemoveAt(pending.Count - 1);
            region.Cells.Add(cell);
            foreach (var neighbour in Neighbours(grid, cell))
            {
                var isHealthy = grid[neighbour.Row][neighbour.Col] == Healthy;
                var joinsRegion = grid[neighbour.Row][neighbour.Col] == Infected && seen.Add(neighbour);
                if (isHealthy)
                {
                    region.Threatened.Add(neighbour);
                }

                if (joinsRegion)
                {
                    pending.Add(neighbour);
                }
            }
        }

        return region;
    }

    private static IEnumerable<(int Row, int Col)> Neighbours(int[][] grid, (int Row, int Col) cell) =>
        Steps
            .Select(step => (Row: cell.Row + step.Row, Col: cell.Col + step.Col))
            .Where(next => next.Row >= 0 && next.Col >= 0 && next.Row < grid.Length && next.Col < grid[next.Row].Length);

    private sealed record Region(List<(int Row, int Col)> Cells, HashSet<(int Row, int Col)> Threatened);
}
