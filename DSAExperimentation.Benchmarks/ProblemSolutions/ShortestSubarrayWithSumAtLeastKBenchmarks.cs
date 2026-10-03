using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ShortestSubarrayWithSumAtLeastK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestSubarrayWithSumAtLeastKSolution's, the same
// methods ShortestSubarrayWithSumAtLeastKSolutionTests proves correct. K is deliberately
// unreachable (values are small and bounded, K is far larger than any possible
// subarray sum) so BOTH strategies are forced through their full worst-case scan
// instead of exiting early on the first short answer found; the answer is LC 862's -1.
public class ShortestSubarrayWithSumAtLeastKBenchmarks
{
    private const int K = 1_000_000;
    private const int ValueLowerBound = -5;
    private const int ValueUpperBoundExclusive = 11;

    private int[] _values = [];

    [Params(400, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = SeededDraws.Values(Length, ValueLowerBound, ValueUpperBoundExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePrefixScan() =>
        ShortestSubarrayWithSumAtLeastKSolution.ShortestSubarrayByBruteForcePrefixScan(_values, K);

    [Benchmark]
    public int MonotonicDequePrefixScan() =>
        ShortestSubarrayWithSumAtLeastKSolution.ShortestSubarrayByMonotonicDeque(_values, K);
}
