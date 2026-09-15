namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3286 - a square 0/1 grid within the problem's
// own 1 <= m, n <= 50 bound. About a third of cells are unsafe, dense enough that
// a shortest "fewest unsafe cells" path has to route around real obstacles rather
// than walk a mostly-zero grid in a straight line.
internal static class SafeWalkGridWorkloads
{
    private const int UnsafeCellPercentChance = 33;

    public static int[][] BuildGrid(int size, int seed)
    {
        var random = new Random(seed);
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];

            for (var col = 0; col < size; col++)
            {
                var isUnsafe = random.Next(100) < UnsafeCellPercentChance;
                grid[row][col] = isUnsafe ? 1 : 0;
            }
        }

        return grid;
    }
}
