using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.KthSmallestInstructions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthSmallestInstructionsSolution's, the same methods
// KthSmallestInstructionsSolutionTests proves correct. The destination and the median rank
// are prepared in [GlobalSetup] so the factorial-sized rank arithmetic is not
// charged to either measured method - what is measured is enumerating C(2n, n)
// routes against deciding 2n characters from cached binomial counts.
//
// Sizes are per arm. Enumerating C(2n, n) routes stops at an 8 x 8 destination; the
// memoized greedy runs on to LC 1643's own bound of 15 x 15, and the two are compared at
// the sizes both run.
public class KthSmallestInstructionsBenchmarks
{
    private Dictionary<int, (int[] Destination, long Rank)> _routeBySize = [];

    public static IEnumerable<int> EnumerateSizes => [5, 8];

    public static IEnumerable<int> MemoizedGreedySizes => [.. EnumerateSizes, 12, 15];

    // Every size any arm runs has its destination and median rank prepared here, outside the
    // timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _routeBySize = MemoizedGreedySizes.ToDictionary(
            size => size,
            size => (
                KthSmallestInstructionsWorkloads.SquareDestination(size),
                KthSmallestInstructionsWorkloads.MedianRank(size)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(EnumerateSizes))]
    public string EnumerateAndSort(int size)
    {
        var (destination, rank) = _routeBySize[size];

        return KthSmallestInstructionsSolution.KthSmallestPathByEnumerateAndSort(destination, rank);
    }

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedGreedySizes))]
    public string MemoizedGreedy(int size)
    {
        var (destination, rank) = _routeBySize[size];

        return KthSmallestInstructionsSolution.KthSmallestPathByMemoizedGreedy(destination, rank);
    }
}
