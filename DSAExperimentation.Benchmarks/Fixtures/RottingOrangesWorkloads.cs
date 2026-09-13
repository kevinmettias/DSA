namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 994 - a square grid whose cells are drawn one in
// ten rotten, one in ten empty, and the rest fresh, with the top-left corner forced
// rotten so the sweep always has a source. That density keeps the fresh oranges
// mostly reachable, which is what makes the baseline pay for a BFS per cell.
internal static class RottingOrangesWorkloads
{
    // random.Next(0, CellStateRoll) weights the rotten/empty/fresh distribution.
    private const int CellStateRoll = 10;
    private const int EmptyCellState = 0;
    private const int FreshOrangeState = 1;
    private const int RottenOrangeState = 2;

    public static int[][] BuildGrid(int size, int seed)
    {
        var random = new Random(seed);
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];

            for (var col = 0; col < size; col++)
            {
                grid[row][col] = random.Next(0, CellStateRoll) switch
                {
                    0 => RottenOrangeState,
                    1 => EmptyCellState,
                    _ => FreshOrangeState,
                };
            }
        }

        grid[0][0] = RottenOrangeState;

        return grid;
    }
}
