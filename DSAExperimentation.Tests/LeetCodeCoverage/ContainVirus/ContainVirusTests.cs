using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainVirus;

// LeetCode 749. Contain Virus: each round's infected regions are 4-connected
// components of 1-cells - the same DepthFirstSearch.Traverse flood fill
// MaxAreaOfIslandTests/NumberOfIslandsTests already use for LC 695/200, run fresh
// every round since both quarantining and spreading mutate the grid between rounds.
// The round-by-round region ranking, wall counting, and quarantine/spread
// bookkeeping around that flood fill is this problem's own simulation logic, not a
// data-structure gap - the same "bespoke control flow around a reused primitive"
// shape SurroundedRegionsTests already has for its own border flood fill.
public sealed partial class ContainVirusTests
{
    [Fact]
    public void ContainVirus_RingSurroundingASingleCell_ReturnsFourWalls()
    {
        int[][] grid = [[1, 1, 1], [1, 0, 1], [1, 1, 1]];

        var walls = MinimumWalls(grid);

        Assert.Equal(4, walls);
    }

    [Fact]
    public void ContainVirus_TwoRegionsRacingForTheOpenMiddle_ReturnsTenWalls()
    {
        int[][] grid =
        [
            [0, 1, 0, 0, 0, 0, 0, 1],
            [0, 1, 0, 0, 0, 0, 0, 1],
            [0, 0, 0, 0, 0, 0, 0, 1],
            [0, 0, 0, 0, 0, 0, 0, 0],
        ];

        var walls = MinimumWalls(grid);

        Assert.Equal(10, walls);
    }

    [Fact]
    public void ContainVirus_IsolatedCellWithNoNeighbors_NeverThreatensAnyoneAndReturnsZero()
    {
        int[][] grid = [[1]];

        var walls = MinimumWalls(grid);

        Assert.Equal(0, walls);
    }

    private static int MinimumWalls(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var totalWalls = 0;

        while (true)
        {
            var wallsThisRound = RunQuarantineRound(grid, rows, cols);

            if (wallsThisRound is null)
            {
                return totalWalls;
            }

            totalWalls += wallsThisRound.Value;
        }
    }

    private static int? RunQuarantineRound(int[][] grid, int rows, int cols)
    {
        var regions = FindRegions(grid, rows, cols);
        var mostThreatening = regions.MaxBy(region => region.Threatened.Count);

        if (mostThreatening is null || mostThreatening.Threatened.Count == 0)
        {
            return null;
        }

        Quarantine(grid, mostThreatening);

        foreach (var region in regions)
        {
            if (region != mostThreatening)
            {
                Spread(grid, region);
            }
        }

        return mostThreatening.WallsNeeded;
    }

    private static List<Region> FindRegions(int[][] grid, int rows, int cols)
    {
        var visited = new HashSet<(int Row, int Col)>();
        var regions = new List<Region>();
        var dimensions = new GridDimensions(rows, cols);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var region = TryBuildRegionAt(grid, dimensions, (r, c), visited);

                if (region is not null)
                {
                    regions.Add(region);
                }
            }
        }

        return regions;
    }

    private static Region? TryBuildRegionAt(
        int[][] grid, GridDimensions dimensions, (int Row, int Col) start, HashSet<(int Row, int Col)> visited)
    {
        var (rows, cols) = dimensions;

        if (grid[start.Row][start.Col] != 1 || !visited.Add(start))
        {
            return null;
        }

        var cells = DepthFirstSearch.Traverse(start, cell => InfectedNeighbors(grid, rows, cols, cell));

        foreach (var cell in cells)
        {
            visited.Add(cell);
        }

        return BuildRegion(grid, rows, cols, cells);
    }

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

    private static void Quarantine(int[][] grid, Region region)
    {
        foreach (var (row, col) in region.Cells)
        {
            grid[row][col] = -1;
        }
    }

    private static void Spread(int[][] grid, Region region)
    {
        foreach (var (row, col) in region.Threatened)
        {
            grid[row][col] = 1;
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

    private readonly record struct GridDimensions(int Rows, int Cols);

    // A plain class, not a record: MaxBy/!= below rely on reference identity to
    // single out "the region just quarantined" among this round's regions, and a
    // record's structural equality would compare List<T>/HashSet<T> fields by
    // reference anyway (neither overrides Equals) - a class states that plainly
    // instead of leaning on an accidental byproduct of record equality.
    private sealed class Region(List<(int Row, int Col)> cells, HashSet<(int Row, int Col)> threatened, int wallsNeeded)
    {
        public List<(int Row, int Col)> Cells { get; } = cells;

        public HashSet<(int Row, int Col)> Threatened { get; } = threatened;

        public int WallsNeeded { get; } = wallsNeeded;
    }
}
