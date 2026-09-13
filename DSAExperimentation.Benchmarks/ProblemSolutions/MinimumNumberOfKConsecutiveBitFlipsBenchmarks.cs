using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfKConsecutiveBitFlips;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfKConsecutiveBitFlipsSolution's, the
// same methods MinimumNumberOfKConsecutiveBitFlipsTests proves correct - the
// textbook rewrite of each k-length window (O(n*k)) against the single sweep whose
// active-flip parity comes from this repo's own Queue<TElement> (O(n), with the
// queue never holding more than k entries).
[MemoryDiagnoser]
public class MinimumNumberOfKConsecutiveBitFlipsBenchmarks
{
    private const int K = 300;

    private const int BitValueUpperBound = 2;

    // Fixed so every run measures the same bit pattern.
    private const int BitSeed = 1;

    [Params(3_000, 30_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(BitSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(BitValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InPlaceWindowFlip() =>
        MinimumNumberOfKConsecutiveBitFlipsSolution.MinKBitFlipsByInPlaceWindowFlip(_nums, K);

    [Benchmark]
    public int QueueTrackedParity() =>
        MinimumNumberOfKConsecutiveBitFlipsSolution.MinKBitFlipsByQueueTrackedParity(_nums, K);
}
