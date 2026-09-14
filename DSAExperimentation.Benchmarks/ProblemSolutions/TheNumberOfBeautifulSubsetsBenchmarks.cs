using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TheNumberOfBeautifulSubsets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TheNumberOfBeautifulSubsetsSolution's, the same
// methods TheNumberOfBeautifulSubsetsTests proves correct. Generate-then-filter
// walks all 2^n bitmasks and checks every C(size,2) pair afterwards; the pruned
// search folds the same rule into Candidates so an illegal inclusion is never made
// and the branch dies immediately. The measured input is built in [GlobalSetup],
// which is already LeetCode's own argument shape, so no hoisted overload is needed.
[MemoryDiagnoser]
public class TheNumberOfBeautifulSubsetsBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 2597;

    private const int MaxValueExclusive = 50;
    private const int K = 3;

    [Params(12, 16)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int GenerateThenFilter() =>
        TheNumberOfBeautifulSubsetsSolution.CountBeautifulSubsetsByBitmask(_nums, K);

    [Benchmark]
    public int PrunedBacktracking() =>
        TheNumberOfBeautifulSubsetsSolution.CountBeautifulSubsetsByPrunedBacktracking(_nums, K);
}
