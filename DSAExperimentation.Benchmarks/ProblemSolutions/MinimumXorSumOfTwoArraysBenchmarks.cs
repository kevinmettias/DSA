using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumXorSumOfTwoArrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumXorSumOfTwoArraysSolution's, the same methods
// MinimumXorSumOfTwoArraysSolutionTests proves correct - the textbook unmemoized bitmask
// recursion over (index, claimedMask), which re-explores that state once per
// assignment ordering that reaches it, against the same recurrence routed through
// this repo's own Memoizer. Both arrays are LeetCode's own input shape, so
// [GlobalSetup] only picks the sizes and the seed.
public class MinimumXorSumOfTwoArraysBenchmarks
{
    // 1879 is the LC problem number.
    private const int RandomSeed = 1879;
    private const int MaxValueBitWidth = 16;

    private int[] _nums1 = [];

    private int[] _nums2 = [];
    [Params(4, 7)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = SeededDraws.Values(Length, 0, 1 << MaxValueBitWidth, random);
        _nums2 = SeededDraws.Values(Length, 0, 1 << MaxValueBitWidth, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        MinimumXorSumOfTwoArraysSolution.MinimumXorSumByBruteForceRecursion(_nums1, _nums2);

    [Benchmark]
    public int MemoizedRecursion() =>
        MinimumXorSumOfTwoArraysSolution.MinimumXorSumByMemoizedBitmask(_nums1, _nums2);
}
