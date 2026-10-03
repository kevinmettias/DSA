using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LargestComponentSizeByCommonFactor;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LargestComponentSizeByCommonFactorSolution's, the same
// methods LargestComponentSizeByCommonFactorSolutionTests proves correct. Values are drawn once
// in [GlobalSetup] by LargestComponentSizeByCommonFactorWorkloads: distinct values built only
// from a small shared prime pool, so real overlaps - and therefore real merge work - actually
// occur. What is measured is the O(n^2) pairwise gcd sweep against the
// O(n*sqrt(maxValue)) per-factor union.
public class LargestComponentSizeByCommonFactorBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 952;

    private int[] _values = [];

    [Params(50, 400)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = LargestComponentSizeByCommonFactorWorkloads.Build(Length, RandomSeed);

    [Benchmark(Baseline = true)]
    public int PairwiseGcdScan() =>
        LargestComponentSizeByCommonFactorSolution.LargestComponentSizeByPairwiseGcd(_values);

    [Benchmark]
    public int DisjointSetByPrimeFactor() =>
        LargestComponentSizeByCommonFactorSolution.LargestComponentSizeByPrimeFactorUnion(_values);
}
