using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumXorSumOfTwoArrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumXorSumOfTwoArraysSolution's, the same methods
// MinimumXorSumOfTwoArraysSolutionTests proves correct - the textbook unmemoized bitmask
// recursion over (index, claimedMask), which re-explores that state once per
// assignment ordering that reaches it, against the same recurrence routed through
// this repo's own Memoizer. Both arrays are LeetCode's own input shape, so
// [GlobalSetup] only picks the sizes and the seed.
//
// Sizes are per arm. The unmemoized recursion walks all n! assignments, so it stops at
// 7; the memoized arm visits each of the 2^n claimed masks once and runs on to
// LC 1879's own bound of 14. The two are compared at the sizes both run.
public class MinimumXorSumOfTwoArraysBenchmarks
{
    // 1879 is the LC problem number.
    private const int RandomSeed = 1879;
    private const int MaxValueBitWidth = 16;

    private Dictionary<int, (int[] Nums1, int[] Nums2)> _arraysBySize = [];

    public static IEnumerable<int> BaselineSizes => [4, 7];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 10, 14];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _arraysBySize = MemoizedSizes.ToDictionary(length => length, BuildArrays);

    private static (int[] Nums1, int[] Nums2) BuildArrays(int length)
    {
        var random = new Random(RandomSeed);
        var nums1 = SeededDraws.Values(length, 0, 1 << MaxValueBitWidth, random);

        return (nums1, SeededDraws.Values(length, 0, 1 << MaxValueBitWidth, random));
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForceRecursion(int length)
    {
        var (nums1, nums2) = _arraysBySize[length];

        return MinimumXorSumOfTwoArraysSolution.MinimumXorSumByBruteForceRecursion(nums1, nums2);
    }

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int length)
    {
        var (nums1, nums2) = _arraysBySize[length];

        return MinimumXorSumOfTwoArraysSolution.MinimumXorSumByMemoizedBitmask(nums1, nums2);
    }
}
