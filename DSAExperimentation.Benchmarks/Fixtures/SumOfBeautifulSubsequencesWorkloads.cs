namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3671 - kept small enough (n <= 16) that the
// brute-force arm's 2^n subsequence enumeration still finishes in a benchmark
// iteration; the divisor-sieve arm's own asymptotic edge only shows up at inputs
// far larger than a bitmask baseline could ever join it at.
internal static class SumOfBeautifulSubsequencesWorkloads
{
    private const int ValueUpperBound = 1_000;

    public static int[] BuildNums(int size, int seed)
    {
        var random = new Random(seed);
        var nums = new int[size];

        for (var i = 0; i < size; i++)
        {
            nums[i] = 1 + random.Next(ValueUpperBound);
        }

        return nums;
    }
}
