namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 703 - a fixed-seed stream of Add values, so both
// strategies process the same interleaved insert sequence.
internal static class KthLargestElementInAStreamWorkloads
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
