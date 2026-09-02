namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3128 - a square 0/1 grid at a fixed one-density,
// the same shape DigitGridWorkloads uses for its own random square grid (LC 3044),
// just over {0,1} instead of digits 1-9.
internal static class RightTriangleWorkloads
{
    private const double OneDensity = 0.3;

    public static int[][] BuildGrid(int size, int seed)
    {
        var random = new Random(seed);
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];

            for (var col = 0; col < size; col++)
            {
                grid[row][col] = random.NextDouble() < OneDensity ? 1 : 0;
            }
        }

        return grid;
    }
}
