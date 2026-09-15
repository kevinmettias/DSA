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

    // Both strategies are stateless, so one instance each serves every call and the
    // benchmark arms that measure these two methods allocate nothing to pick one.
    private static readonly IRegionCollector NaiveRecursiveFloodFill = new RegionByNaiveRecursiveFloodFill();
    private static readonly IRegionCollector DepthFirstSearchTraversal = new RegionByDepthFirstSearchTraversal();

    // The textbook answer: a hand-specialized recursive flood fill, BCL only.
    // Deliberately written without this repo's traversal primitive - it is the arm
    // the composed solution below has to justify itself against.
    public static int MinimumWallsByNaiveRecursiveFloodFill(int[][] grid) =>
        MinimumWalls(CloneGrid(grid), NaiveRecursiveFloodFill);

    // This repo's own DFS: DepthFirstSearch.Traverse walks one region's full
    // infected component from each unvisited infected cell - the same
    // border-flood-fill primitive MaxAreaOfIslandSolution uses for LC 695.
    public static int MinimumWallsByDepthFirstSearchTraversal(int[][] grid) =>
        MinimumWalls(CloneGrid(grid), DepthFirstSearchTraversal);

    // The one question the two arms answer differently: which cells make up the
    // infected region reachable from `start`. The grid arrives as an argument rather
    // than a captured field because every round mutates it in place, and a returned
    // list holds exactly the start cell plus every infected cell reachable from it by
    // 4-connected steps. An implementation may reach those cells by recursion or by a
    // traversal primitive; it may not return a subset or a superset of them.
    private interface IRegionCollector
    {
        List<(int Row, int Col)> Collect(int[][] grid, (int Row, int Col) start);
    }

    private sealed class RegionByNaiveRecursiveFloodFill : IRegionCollector
    {
        public List<(int Row, int Col)> Collect(int[][] grid, (int Row, int Col) start)
        {
            var cells = new List<(int Row, int Col)>();
            var visited = new HashSet<(int Row, int Col)>();

            Flood(grid, start, visited, cells);

            return cells;
        }
    }

    private sealed class RegionByDepthFirstSearchTraversal : IRegionCollector
    {
        public List<(int Row, int Col)> Collect(int[][] grid, (int Row, int Col) start) =>
            DepthFirstSearch.Traverse(start, cell => InfectedNeighbors(grid, cell));
    }

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

    // A cell joins the region only when it is on the board, infected, and not
    // already visited - `visited.Add` both tests and records the visit, so it
    // has to stay last in the chain.
    private static bool IsUnvisitedInfectedCell(
        (int Row, int Col) cell, int[][] grid, HashSet<(int Row, int Col)> visited)
        => IsInBounds(cell, grid) && grid[cell.Row][cell.Col] == 1 && visited.Add(cell);

    private static void Flood(
        int[][] grid, (int Row, int Col) cell, HashSet<(int Row, int Col)> visited, List<(int Row, int Col)> cells)
    {
        if (!IsUnvisitedInfectedCell(cell, grid, visited))
        {
            return;
        }

        cells.Add(cell);

        foreach (var (dRow, dCol) in Directions)
        {
            Flood(grid, (cell.Row + dRow, cell.Col + dCol), visited, cells);
        }
    }

    // The simulation shared by both strategies: find every infected region, wall
    // off whichever threatens the most uninfected cells, let the rest spread.
    private static int MinimumWalls(int[][] grid, IRegionCollector collectRegion)
    {
        var totalWalls = 0;

        // Terminates when a round finds nothing left to threaten: RunQuarantineRound
        // reports that by returning null, and the accumulated wall count is the answer.
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
        int[][] grid, IRegionCollector collectRegion)
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
        int[][] grid, IRegionCollector collectRegion)
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
        IRegionCollector collectRegion)
    {
        if (grid[start.Row][start.Col] != 1 || !visited.Add(start))
        {
            return null;
        }

        var cells = collectRegion.Collect(grid, start);

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

    // Pure data, so a record. Its generated equality compares Cells and Threatened by
    // reference (neither List<T> nor HashSet<T> overrides Equals), and BuildRegion is
    // the only place a Region is built - always with its own freshly allocated HashSet
    // - so two distinct regions can never compare equal, and the `region != mostThreatening`
    // in RunQuarantineRound still excludes exactly the region just quarantined.
    private sealed record Region(
        List<(int Row, int Col)> Cells,
        HashSet<(int Row, int Col)> Threatened,
        int WallsNeeded);
}
