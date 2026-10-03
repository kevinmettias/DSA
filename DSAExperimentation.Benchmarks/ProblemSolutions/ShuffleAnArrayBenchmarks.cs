using DSAExperimentation.LeetCode.ShuffleAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShuffleAnArraySolution's, the same methods
// ShuffleAnArraySolutionTests proves correct. The naive "remove a random remaining
// element" baseline (a List<int>.RemoveAt shifts every trailing element on almost
// every draw - O(n^2) worst case) vs. in-place Fisher-Yates over this repo's own
// DynamicArray<int> - O(n), no auxiliary "remaining pool" collection at all.
// Each call seeds a fresh Random rather than sharing one built in [GlobalSetup]:
// the strategy advances the generator it is handed, so a shared one would draw a
// different shuffle every iteration. Seeding it is timed on purpose, and every arm
// pays the same cost.
public class ShuffleAnArrayBenchmarks
{
    private int[] _original = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _original = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int[] RemoveRandomRemaining() =>
        ShuffleAnArraySolution.ShuffleByRemoveRandomRemaining(_original, new Random(1));

    [Benchmark]
    public int[] FisherYatesDynamicArray() =>
        ShuffleAnArraySolution.ShuffleByFisherYatesDynamicArray(_original, new Random(1));
}
