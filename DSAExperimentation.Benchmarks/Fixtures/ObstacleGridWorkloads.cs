namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2290 - a square 0/1 grid where roughly one
// cell in five is an obstacle, dense enough that the cheapest route has to weave
// rather than run straight down the first row and across the last column. Both
// corners are cleared because the problem guarantees them clear.
internal static class ObstacleGridWorkloads
{
    private const int ObstacleOneIn = 5;

    public static int[][] BuildGrid(int size, int seed)
    {
        var random = new Random(seed);
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];

            for (var col = 0; col < size; col++)
            {
                var isObstacle = random.Next(ObstacleOneIn) == 0;
                grid[row][col] = isObstacle ? 1 : 0;
            }
        }

        grid[0][0] = 0;
        grid[size - 1][size - 1] = 0;

        return grid;
    }
}
