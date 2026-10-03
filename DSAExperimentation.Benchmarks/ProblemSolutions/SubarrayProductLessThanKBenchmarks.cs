using DSAExperimentation.LeetCode.SubarrayProductLessThanK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubarrayProductLessThanKSolution's, the same methods
// SubarrayProductLessThanKSolutionTests proves correct. nums are deliberately all 1s
// (product never reaches K) so BruteForce's inner loop never breaks early and is
// forced through its real O(n^2) worst case.
public class SubarrayProductLessThanKBenchmarks
{
    private const int K = 2;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = Enumerable.Repeat(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        SubarrayProductLessThanKSolution.CountSubarraysWithProductLessThanKByBruteForce(_nums, K);

    [Benchmark]
    public int LogPrefixLowerBound() =>
        SubarrayProductLessThanKSolution.CountSubarraysWithProductLessThanKByLogPrefixLowerBound(_nums, K);
}
