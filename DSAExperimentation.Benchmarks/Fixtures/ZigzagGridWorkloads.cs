namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3417 - a rectangular grid of positive values
// within the problem's own 1 <= grid[i][j] <= 2500 bound, sized well within its
// 2 <= m, n <= 50 constraint so both strategies still traverse the exact shape
// LeetCode's judge would present them.
internal static class ZigzagGridWorkloads
{
    public static int[][] BuildGrid(int rows, int cols, int seed)
    {
        var random = new Random(seed);
        var grid = new int[rows][];

        for (var row = 0; row < rows; row++)
        {
            grid[row] = new int[cols];

            for (var col = 0; col < cols; col++)
            {
                grid[row][col] = random.Next(1, 2501);
            }
        }

        return grid;
    }
}
