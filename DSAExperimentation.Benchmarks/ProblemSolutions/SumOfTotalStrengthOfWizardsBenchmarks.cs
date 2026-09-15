using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfTotalStrengthOfWizards;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfTotalStrengthOfWizardsSolution's, the same
// methods SumOfTotalStrengthOfWizardsTests proves correct. _strength is a random
// permutation so the brute-force arm's inner loop always runs its full remaining
// length - with every value distinct there is no run of equal minimums to let it
// settle early.
[MemoryDiagnoser]
public class SumOfTotalStrengthOfWizardsBenchmarks
{
    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 2281;

    private int[] _strength = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _strength = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => SumOfTotalStrengthOfWizardsSolution.TotalStrengthByBruteForce(_strength);

    [Benchmark]
    public int MonotonicStackContribution() =>
        SumOfTotalStrengthOfWizardsSolution.TotalStrengthByMonotonicStack(_strength);
}
