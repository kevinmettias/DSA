namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3044 - a square grid of digits 1-9 (mat's
// own constraint, so no leading-zero cells), sized well within the problem's
// 1 <= m, n <= 6 bound so both strategies still search the exact same
// combinatorial space LeetCode's judge would present them.
internal static class DigitGridWorkloads
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
                grid[row][col] = random.Next(1, 10);
            }
        }

        return grid;
    }
}
