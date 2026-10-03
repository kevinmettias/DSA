using DSAExperimentation.LeetCode.NumberOfWaysToRearrangeSticksWithKSticksVisible;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution's, the same methods its
// test proves correct. The comparison is the textbook O(n!) enumeration of every
// arrangement against this repo's own Memoizer computing the identical count via
// the unsigned-Stirling-number recurrence in O(StickCount*VisibleCount) states.
//
// Sizes are per arm. The brute force's factorial blowup is real, so it stops at 9
// sticks (the same reason StoneGameVIIBenchmarks keeps its own exponential baseline
// modest); the Stirling recurrence runs on to LC 1866's own bound of 1,000. The two
// are compared at the sizes both run.
public class NumberOfWaysToRearrangeSticksWithKSticksVisibleBenchmarks
{
    private const int VisibleCount = 3;

    public static IEnumerable<int> BaselineSizes => [8, 9];

    public static IEnumerable<int> StirlingSizes => [.. BaselineSizes, 100, 1_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForcePermutations(int stickCount) =>
        NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
            .RearrangeSticksByPermutationEnumeration(stickCount, VisibleCount);

    [Benchmark]
    [ArgumentsSource(nameof(StirlingSizes))]
    public int MemoizedStirlingRecurrence(int stickCount) =>
        NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
            .RearrangeSticksByMemoizedStirling(stickCount, VisibleCount);
}
