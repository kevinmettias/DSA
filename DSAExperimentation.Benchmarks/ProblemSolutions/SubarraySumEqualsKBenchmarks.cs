using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubarraySumEqualsK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubarraySumEqualsKSolution's, the same methods
// SubarraySumEqualsKTests proves correct - the canonical O(n^2) brute-force double
// loop vs. the O(n) single pass tracking prefix-sum frequency in this repo's own
// HashMap<int,int>. Target is deliberately unreachable (values are small and
// bounded, target is far outside any possible running sum) so both strategies are
// forced through their full worst-case scan instead of an early exit favoring one
// of them.
[MemoryDiagnoser]
public class SubarraySumEqualsKBenchmarks
{
    private const int Target = 1_000_000;
    private const int ValueLowerBound = -10;
    private const int ValueUpperBoundExclusive = 11;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(ValueLowerBound, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => SubarraySumEqualsKSolution.CountByBruteForce(_values, Target);

    [Benchmark]
    public int PrefixSumHashMap() => SubarraySumEqualsKSolution.CountByPrefixSumHashMap(_values, Target);
}
