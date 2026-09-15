namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 200 - roughly half the cells are land ('1')
// scattered at random, so the flood fill has to start many separate islands
// instead of walking one solid block.
internal static class NumberOfIslandsWorkloads
{
    private const int LandChance = 2;

    public static char[][] BuildGrid(int rows, int cols, int seed)
    {
        var random = new Random(seed);
        var grid = new char[rows][];

        for (var row = 0; row < rows; row++)
        {
            grid[row] = new char[cols];

            for (var col = 0; col < cols; col++)
            {
                var isLand = random.Next(LandChance) == 0;
                grid[row][col] = isLand ? '1' : '0';
            }
        }

        return grid;
    }
}
