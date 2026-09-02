namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3493 - each row is m values drawn from LC's
// own [1, 100] value range, at a column count well under it, so pairs land a mix
// of above- and below-threshold intersections instead of either every pair
// missing entirely or the whole graph trivially collapsing into one component.
internal static class PropertiesGraphWorkloads
{
    private const int ValueRange = 100;

    public static int[][] BuildProperties(int rowCount, int columnCount, int seed)
    {
        var random = new Random(seed);
        var properties = new int[rowCount][];

        for (var i = 0; i < rowCount; i++)
        {
            var row = new int[columnCount];

            for (var j = 0; j < columnCount; j++)
            {
                row[j] = random.Next(1, ValueRange + 1);
            }

            properties[i] = row;
        }

        return properties;
    }
}
