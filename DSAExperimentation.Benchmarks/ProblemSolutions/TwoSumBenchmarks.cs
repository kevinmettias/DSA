using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TwoSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TwoSumSolution's, the same methods TwoSumTests
// proves correct. Target is deliberately unreachable (all values positive, target
// negative) so BOTH strategies are forced through their full worst-case scan
// instead of an early exit making brute force look artificially competitive.
[MemoryDiagnoser]
public class TwoSumBenchmarks
{
    private const int Target = -1;
    private const int MaxValueExclusive = 1_000;
    private const int Seed = 1;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BruteForce() => TwoSumSolution.TryFindIndicesByBruteForce(_values, Target, out _, out _);

    [Benchmark]
    public bool HashMapOnePass() => TwoSumSolution.TryFindIndicesByHashMap(_values, Target, out _, out _);
}
