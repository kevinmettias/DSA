using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.ContainVirus;

// LeetCode 749. Contain Virus: each round's infected regions are 4-connected
// components of 1-cells - the same flood fill MaxAreaOfIslandSolution already uses
// for LC 695. Both strategies run the identical round-by-round simulation (find
// regions, rank by threat, quarantine the worst, spread the rest) and differ only
// in how a region's cells are collected; regions are rediscovered fresh every
// round since quarantining and spreading both mutate the grid. Both strategies
// clone the grid they are handed first, so a caller's own grid (or a benchmark
// fixture reused across iterations) is never left half-quarantined.
internal static class ContainVirusSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook answer: a hand-specialized recursive flood fill, BCL only.
    // Deliberately written without this repo's traversal primitive - it is the arm
    // the composed solution below has to justify itself against.
    public static int MinimumWallsByNaiveRecursiveFloodFill(int[][] grid) =>
        MinimumWalls(CloneGrid(grid), CollectRegionRecursive);

    private static List<(int Row, int Col)> CollectRegionRecursive(int[][] grid, (int Row, int Col) start)
    {
        var cells = new List<(int Row, int Col)>();
        var visited = new HashSet<(int Row, int Col)>();

        Flood(grid, start, visited, cells);

        return cells;
    }

    private static void Flood(
        int[][] grid, (int Row, int Col) cell, HashSet<(int Row, int Col)> visited, List<(int Row, int Col)> cells)
    {
        if (!IsInBounds(cell, grid) || grid[cell.Row][cell.Col] != 1 || !visited.Add(cell))
        {
            return;
        }

        cells.Add(cell);

        foreach (var (dRow, dCol) in Directions)
        {
            Flood(grid, (cell.Row + dRow, cell.Col + dCol), visited, cells);
        }
    }

    // This repo's own DFS: DepthFirstSearch.Traverse walks one region's full
    // infected component from each unvisited infected cell - the same
    // border-flood-fill primitive MaxAreaOfIslandSolution uses for LC 695.
    public static int MinimumWallsByDepthFirstSearchTraversal(int[][] grid) =>
        MinimumWalls(CloneGrid(grid), CollectRegionViaTraversal);

    private static List<(int Row, int Col)> CollectRegionViaTraversal(int[][] grid, (int Row, int Col) start) =>
        DepthFirstSearch.Traverse(start, cell => InfectedNeighbors(grid, cell));

    private static IEnumerable<(int Row, int Col)> InfectedNeighbors(int[][] grid, (int Row, int Col) cell)
    {
        foreach (var neighbor in FourNeighbors(cell, grid))
        {
            if (grid[neighbor.Row][neighbor.Col] == 1)
            {
                yield return neighbor;
            }
        }
    }

    // The simulation shared by both strategies: find every infected region, wall
    // off whichever threatens the most uninfected cells, let the rest spread.
    private static int MinimumWalls(int[][] grid, Func<int[][], (int Row, int Col), List<(int Row, int Col)>> collectRegion)
    {
        var totalWalls = 0;

        while (true)
        {
            var wallsThisRound = RunQuarantineRound(grid, collectRegion);

            if (wallsThisRound is null)
            {
                return totalWalls;
            }

            totalWalls += wallsThisRound.Value;
        }
    }

    private static int? RunQuarantineRound(
        int[][] grid, Func<int[][], (int Row, int Col), List<(int Row, int Col)>> collectRegion)
    {
        var regions = FindRegions(grid, collectRegion);
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

    private static List<Region> FindRegions(
        int[][] grid, Func<int[][], (int Row, int Col), List<(int Row, int Col)>> collectRegion)
    {
        var visited = new HashSet<(int Row, int Col)>();
        var regions = new List<Region>();

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                var region = TryBuildRegionAt(grid, (r, c), visited, collectRegion);

                if (region is not null)
                {
                    regions.Add(region);
                }
            }
        }

        return regions;
    }

    private static Region? TryBuildRegionAt(
        int[][] grid,
        (int Row, int Col) start,
        HashSet<(int Row, int Col)> visited,
        Func<int[][], (int Row, int Col), List<(int Row, int Col)>> collectRegion)
    {
        if (grid[start.Row][start.Col] != 1 || !visited.Add(start))
        {
            return null;
        }

        var cells = collectRegion(grid, start);

        foreach (var cell in cells)
        {
            visited.Add(cell);
        }

        return BuildRegion(grid, cells);
    }

    private static Region BuildRegion(int[][] grid, List<(int Row, int Col)> cells)
    {
        var threatened = new HashSet<(int Row, int Col)>();
        var wallsNeeded = 0;

        foreach (var cell in cells)
        {
            foreach (var neighbor in FourNeighbors(cell, grid))
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

    private static IEnumerable<(int Row, int Col)> FourNeighbors((int Row, int Col) cell, int[][] grid)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var candidate = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (IsInBounds(candidate, grid))
            {
                yield return candidate;
            }
        }
    }

    private static bool IsInBounds((int Row, int Col) cell, int[][] grid) =>
        cell.Row >= 0 && cell.Row < grid.Length && cell.Col >= 0 && cell.Col < grid[0].Length;

    private static int[][] CloneGrid(int[][] grid)
    {
        var clone = new int[grid.Length][];

        for (var r = 0; r < grid.Length; r++)
        {
            clone[r] = (int[])grid[r].Clone();
        }

        return clone;
    }

    // A plain class, not a record: MaxBy/!= rely on reference identity to single
    // out "the region just quarantined" among this round's regions, and a record's
    // structural equality would compare List<T>/HashSet<T> fields by reference
    // anyway (neither overrides Equals) - a class states that plainly instead of
    // leaning on an accidental byproduct of record equality.
    private sealed class Region(List<(int Row, int Col)> cells, HashSet<(int Row, int Col)> threatened, int wallsNeeded)
    {
        public List<(int Row, int Col)> Cells { get; } = cells;

        public HashSet<(int Row, int Col)> Threatened { get; } = threatened;

        public int WallsNeeded { get; } = wallsNeeded;
    }
}
