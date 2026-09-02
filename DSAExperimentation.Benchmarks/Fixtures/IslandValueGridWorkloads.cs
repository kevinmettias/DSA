namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3619 - roughly half the cells are water (0)
// and half are land with a random positive value, so both strategies flood-fill
// many separate islands instead of walking one solid block.
internal static class IslandValueGridWorkloads
{
    private const int LandValueUpperBound = 1_000_000;
    private const int WaterChance = 2;

    public static int[][] BuildGrid(int rows, int cols, int seed)
    {
        var random = new Random(seed);
        var grid = new int[rows][];

        for (var row = 0; row < rows; row++)
        {
            grid[row] = new int[cols];

            for (var col = 0; col < cols; col++)
            {
                grid[row][col] = random.Next(WaterChance) == 0 ? 0 : random.Next(1, LandValueUpperBound);
            }
        }

        return grid;
    }
}
