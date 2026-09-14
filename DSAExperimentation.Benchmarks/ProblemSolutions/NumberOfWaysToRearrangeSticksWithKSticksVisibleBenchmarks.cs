using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfWaysToRearrangeSticksWithKSticksVisible;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution's, the same methods its
// test proves correct. The comparison is the textbook O(n!) enumeration of every
// arrangement against this repo's own Memoizer computing the identical count via
// the unsigned-Stirling-number recurrence in O(StickCount*VisibleCount) states.
// StickCount is kept modest for the same reason StoneGameVIIBenchmarks keeps
// PileCount modest for its own exponential baseline - the brute force's factorial
// blowup is real.
[MemoryDiagnoser]
public class NumberOfWaysToRearrangeSticksWithKSticksVisibleBenchmarks
{
    private const int VisibleCount = 3;

    [Params(8, 9)]
    public int StickCount;

    [Benchmark(Baseline = true)]
    public int BruteForcePermutations() =>
        NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
            .RearrangeSticksByPermutationEnumeration(StickCount, VisibleCount);

    [Benchmark]
    public int MemoizedStirlingRecurrence() =>
        NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
            .RearrangeSticksByMemoizedStirling(StickCount, VisibleCount);
}
