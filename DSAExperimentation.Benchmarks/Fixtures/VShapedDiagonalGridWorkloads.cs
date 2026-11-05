namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3459 - a random square grid over {0,1,2}, each cell
// drawn uniformly from the three. 0 is a valid mid-segment value here, not a blocked
// cell, so an even split leaves 1-starts and their 2/0 follow-ups common enough for
// both strategies to find real segments to extend.
internal static class VShapedDiagonalGridWorkloads
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
                grid[row][col] = random.Next(3);
            }
        }

        return grid;
    }
}
