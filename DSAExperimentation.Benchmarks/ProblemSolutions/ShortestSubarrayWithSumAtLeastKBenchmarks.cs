using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShortestSubarrayWithSumAtLeastK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestSubarrayWithSumAtLeastKSolution's, the same
// methods ShortestSubarrayWithSumAtLeastKTests proves correct. K is deliberately
// unreachable (values are small and bounded, K is far larger than any possible
// subarray sum) so BOTH strategies are forced through their full worst-case scan
// instead of exiting early on the first short answer found - the same "_target is
// deliberately unreachable" shape TwoSumBenchmarks uses.
[MemoryDiagnoser]
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
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(ValueLowerBound, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePrefixScan() =>
        ShortestSubarrayWithSumAtLeastKSolution.ShortestSubarrayByBruteForcePrefixScan(_values, K);

    [Benchmark]
    public int MonotonicDequePrefixScan() =>
        ShortestSubarrayWithSumAtLeastKSolution.ShortestSubarrayByMonotonicDeque(_values, K);
}
