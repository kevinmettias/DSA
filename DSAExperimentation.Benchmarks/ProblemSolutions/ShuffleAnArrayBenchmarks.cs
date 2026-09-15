using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShuffleAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShuffleAnArraySolution's, the same methods
// ShuffleAnArrayTests proves correct. The naive "remove a random remaining
// element" baseline (a List<int>.RemoveAt shifts every trailing element on almost
// every draw - O(n^2) worst case) vs. in-place Fisher-Yates over this repo's own
// DynamicArray<int> - O(n), no auxiliary "remaining pool" collection at all.
[MemoryDiagnoser]
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
