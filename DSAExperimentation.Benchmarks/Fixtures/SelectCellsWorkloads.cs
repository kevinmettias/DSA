namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3276 - a square grid within the problem's own
// 1 <= grid.length, grid[i].length <= 10 bound. Values are drawn from a range
// tied to the grid's own size (rather than the problem's full 1..100) so most
// rows still hold several duplicate values, the case the brute-force baseline's
// row-by-row recursion has to re-derive from scratch every time and the bitmask
// strategy collapses into one memoized state.
internal static class SelectCellsWorkloads
{
    public static int[][] BuildGrid(int size, int seed)
    {
        var random = new Random(seed);
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];

            for (var col = 0; col < size; col++)
            {
                grid[row][col] = random.Next(1, size + 1);
            }
        }

        return grid;
    }
}
