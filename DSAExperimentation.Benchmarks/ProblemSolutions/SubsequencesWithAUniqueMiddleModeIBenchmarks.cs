using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubsequencesWithAUniqueMiddleModeI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubsequencesWithAUniqueMiddleModeISolution's, the
// same methods SubsequencesWithAUniqueMiddleModeITests proves correct. A small
// value range forces plenty of repeats, exercising the modular-combinatorics
// arm's distinct-pair bookkeeping instead of degenerating to the all-values-
// unique case. Length stays small (the brute-force arm is O(n^5)); the
// combinatorics arm scales to LeetCode's real n = 1000 because it is O(n^2).
[MemoryDiagnoser]
public class SubsequencesWithAUniqueMiddleModeIBenchmarks
{
    private const int RandomSeed = 3395; // LC problem number
    private const int ValueRange = 5;

    [Params(10, 14)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(ValueRange)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => SubsequencesWithAUniqueMiddleModeISolution.CountMiddleModeSubsequencesByBruteForce(_nums);

    [Benchmark]
    public long ModularCombinatorics() =>
        SubsequencesWithAUniqueMiddleModeISolution.CountMiddleModeSubsequencesByModularCombinatorics(_nums);
}
