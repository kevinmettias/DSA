namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1093 - a count for each of the 256 sample values, drawn
// from [1, 2 * averageCountPerValue] so the sampled mean lands near averageCountPerValue, with
// one more sample added to the first bucket holding the largest count. Drawn counts tie for
// the largest often at small averages, and LC 1093 promises the sample's mode is unique; the
// extra sample breaks the tie without moving any draw.
internal static class StatisticsFromALargeSampleWorkloads
{
    // LC 1093 fixes the sample values at [0, 255].
    public const int ValueRange = 256;

    private const int MaxCountMultiplier = 2;

    public static long[] BuildCounts(int averageCountPerValue, int seed)
    {
        var random = new Random(seed);
        var counts = new long[ValueRange];

        for (var i = 0; i < ValueRange; i++)
        {
            counts[i] = random.Next(1, (averageCountPerValue * MaxCountMultiplier) + 1);
        }

        var firstLargest = Array.IndexOf(counts, counts.Max());
        counts[firstLargest]++;

        return counts;
    }
}
