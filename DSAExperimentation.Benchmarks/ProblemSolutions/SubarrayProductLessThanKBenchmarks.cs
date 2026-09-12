using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubarrayProductLessThanK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubarrayProductLessThanKSolution's, the same methods
// SubarrayProductLessThanKTests proves correct. nums are deliberately all 1s
// (product never reaches K) so BruteForce's inner loop never breaks early and is
// forced through its real O(n^2) worst case - the same "force the real worst case"
// convention TwoSumBenchmarks (unreachable target) and
// LongestSubstringWithoutRepeatingCharactersBenchmarks (all-distinct characters)
// already establish.
[MemoryDiagnoser]
public class SubarrayProductLessThanKBenchmarks
{
    private const int K = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() => _nums = Enumerable.Repeat(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        SubarrayProductLessThanKSolution.NumSubarrayProductLessThanKByBruteForce(_nums, K);

    [Benchmark]
    public int LogPrefixLowerBound() =>
        SubarrayProductLessThanKSolution.NumSubarrayProductLessThanKByLogPrefixLowerBound(_nums, K);
}
