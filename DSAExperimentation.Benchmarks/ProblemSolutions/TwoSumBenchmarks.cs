using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TwoSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TwoSumSolution's, the same methods TwoSumSolutionTests
// proves correct. TwoSumWorkloads plants the one pair LC 1 promises at the last two
// positions, so BOTH strategies are forced through their full worst-case scan before
// they find it, instead of an early exit making brute force look artificially
// competitive.
public class TwoSumBenchmarks
{
    private const int Seed = 1;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = TwoSumWorkloads.BuildValues(Length, Seed);

    [Benchmark(Baseline = true)]
    public bool TryFindIndicesByBruteForce() => TwoSumSolution.TryFindIndicesByBruteForce(_values, TwoSumWorkloads.Target, out _, out _);

    [Benchmark]
    public bool TryFindIndicesByHashMap() => TwoSumSolution.TryFindIndicesByHashMap(_values, TwoSumWorkloads.Target, out _, out _);
}
