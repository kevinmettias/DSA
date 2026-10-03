using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TwoSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TwoSumSolution's, the same methods TwoSumSolutionTests
// proves correct. Target is deliberately unreachable (all values positive, target
// negative) so BOTH strategies are forced through their full worst-case scan
// instead of an early exit making brute force look artificially competitive.
public class TwoSumBenchmarks
{
    private const int Target = -1;
    private const int MaxValueExclusive = 1_000;
    private const int Seed = 1;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _values = SeededDraws.Values(Length, 1, MaxValueExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public bool TryFindIndicesByBruteForce() => TwoSumSolution.TryFindIndicesByBruteForce(_values, Target, out _, out _);

    [Benchmark]
    public bool TryFindIndicesByHashMap() => TwoSumSolution.TryFindIndicesByHashMap(_values, Target, out _, out _);
}
