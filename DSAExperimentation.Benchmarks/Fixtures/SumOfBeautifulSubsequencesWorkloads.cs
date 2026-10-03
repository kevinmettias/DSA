namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3671 - random values at whatever length an arm
// asks for. The benchmark keeps the brute-force arm's 2^n subsequence enumeration to
// n <= 16, and runs the divisor-sieve arm on to LC 3671's bound of 10^4, where its
// asymptotic edge shows at inputs no bitmask baseline could ever join it at.
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
