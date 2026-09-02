namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 295 (Find Median from Data Stream) - a
// fixed-seed stream of AddNum values, so both strategies process the same
// interleaved AddNum/FindMedian sequence.
internal static class FindMedianFromDataStreamWorkloads
{
    public static int[] BuildStream(int length, int seed, int maxValue)
    {
        var random = new Random(seed);
        var stream = new int[length];

        for (var i = 0; i < length; i++)
        {
            stream[i] = random.Next(1, maxValue);
        }

        return stream;
    }
}
