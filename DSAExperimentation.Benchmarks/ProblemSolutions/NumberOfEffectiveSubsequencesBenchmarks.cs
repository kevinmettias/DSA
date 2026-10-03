using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfEffectiveSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfEffectiveSubsequencesSolution's, the
// same methods NumberOfEffectiveSubsequencesSolutionTests proves correct.
public class NumberOfEffectiveSubsequencesBenchmarks
{
    private const int Seed = 3757;
    private const int MaxValueExclusive = 1_000_000;

    private int[] _nums = [];

    // Kept small: BruteForce is O(n * 2^n), so Length only spans what it can
    // still finish walking every subset for.
    [Params(12, 18)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = SeededDraws.Values(Length, 1, MaxValueExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => NumberOfEffectiveSubsequencesSolution.CountEffectiveByBruteForce(_nums);

    [Benchmark]
    public int OrSubsetTransform() => NumberOfEffectiveSubsequencesSolution.CountEffectiveByOrSubsetTransform(_nums);
}
