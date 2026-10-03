namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 1453: a cloud of integer darts drawn uniformly from a square
// around the origin. LC 1453 promises every dart is unique, so a draw that lands on a
// dart already thrown is drawn again.
internal static class MaximumNumberOfDartsWorkloads
{
    public static int[][] BuildDarts(int count, int coordinateRange, int seed)
    {
        var random = new Random(seed);
        var thrown = new HashSet<(int X, int Y)>();
        var darts = new int[count][];
        var next = 0;

        while (next < count)
        {
            var x = random.Next(-coordinateRange, coordinateRange);
            var y = random.Next(-coordinateRange, coordinateRange);

            if (thrown.Add((x, y)))
            {
                darts[next++] = [x, y];
            }
        }

        return darts;
    }
}
