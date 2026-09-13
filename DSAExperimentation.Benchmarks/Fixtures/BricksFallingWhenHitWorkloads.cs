namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 803 - how large a wall to knock down and how
// much of it to hit, which are measurement decisions. Row 0 is kept fully bricked
// and every other row filled at a fixed density, with hits drawn only from real
// brick positions, so both strategies do real connectivity work on every hit
// instead of mostly no-op ones.
internal static class BricksFallingWhenHitWorkloads
{
    private const double BrickDensity = 0.6;
    private const int HitFractionDivisor = 3;

    // LeetCode's own input shape: a 0/1 grid plus a [row, col] hit list, drawn
    // from one seeded sequence so the wall and the hits against it stay paired.
    public static BrickWall BuildWall(int size, int seed)
    {
        var random = new Random(seed);
        var grid = BuildGrid(random, size);
        var brickPositions = CollectBrickPositions(grid, size);
        var hits = SelectHits(random, brickPositions, size);

        return new BrickWall(grid, hits);
    }

    private static int[][] BuildGrid(Random random, int size)
    {
        var grid = new int[size][];

        for (var r = 0; r < size; r++)
        {
            grid[r] = new int[size];

            for (var c = 0; c < size; c++)
            {
                grid[r][c] = r == 0 || random.NextDouble() < BrickDensity ? 1 : 0;
            }
        }

        return grid;
    }

    private static List<(int Row, int Col)> CollectBrickPositions(int[][] grid, int size)
    {
        var brickPositions = new List<(int Row, int Col)>();

        for (var r = 0; r < size; r++)
        {
            for (var c = 0; c < size; c++)
            {
                if (grid[r][c] == 1)
                {
                    brickPositions.Add((r, c));
                }
            }
        }

        return brickPositions;
    }

    private static int[][] SelectHits(Random random, List<(int Row, int Col)> brickPositions, int size)
    {
        var hitCount = Math.Min(brickPositions.Count, (size * size) / HitFractionDivisor);

        return brickPositions
            .OrderBy(_ => random.Next())
            .Take(hitCount)
            .Select(p => new[] { p.Row, p.Col })
            .ToArray();
    }
}
