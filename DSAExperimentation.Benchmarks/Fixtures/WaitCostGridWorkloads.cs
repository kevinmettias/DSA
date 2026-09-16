namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for the three wait-cost grid siblings - LC 3341, LC 3342
// and LC 2577 - whose grids are built the same way: every cell but the origin demands
// a random wait, so each relaxation goes through the wait (and, for the latter two,
// the parity) branch instead of taking a constant-weight shortcut. The origin is
// pinned to 0 because all three problems guarantee that cell costs nothing to enter.
internal static class WaitCostGridWorkloads
{
    public static int[][] WithZeroOrigin(int size, int highExclusive, int seed)
    {
        var random = new Random(seed);
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];

            for (var col = 0; col < size; col++)
            {
                grid[row][col] = row == 0 && col == 0 ? 0 : random.Next(0, highExclusive);
            }
        }

        return grid;
    }
}
