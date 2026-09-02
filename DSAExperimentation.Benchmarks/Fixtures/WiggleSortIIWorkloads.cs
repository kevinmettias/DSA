namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 324 - values are bounded to length/2 so
// duplicates cluster around the median, which is the case that makes a naive
// ascending interleave invalid and the reversed-half interleave necessary.
internal static class WiggleSortIIWorkloads
{
    public static int[] BuildValuesWithDuplicates(int length, int seed, int valueRangeDivisor)
    {
        var random = new Random(seed);
        var values = new int[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = random.Next(0, length / valueRangeDivisor);
        }

        return values;
    }
}
