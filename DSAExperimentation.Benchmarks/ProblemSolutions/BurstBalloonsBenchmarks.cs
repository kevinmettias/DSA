using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BurstBalloons;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BurstBalloonsSolution's, the same methods
// BurstBalloonsSolutionTests proves correct. Each arm is handed the prepared
// PaddedBalloons its hoisted overload takes, so the boundary-padding pass is
// charged to [GlobalSetup] rather than to the recursion being measured.
//
// Sizes are per arm. The un-memoized baseline is genuinely exponential, so it stops
// at 14 balloons; sharing that cap would leave the memoized arm timed only where its
// O(n^3) growth cannot show. It runs on to LC 312's own bound of 300, and the two are
// compared - and reported as a ratio - at the sizes both run.
public class BurstBalloonsBenchmarks
{
    private const int RandomSeed = 1;

    private Dictionary<int, PaddedBalloons> _paddedBySize = [];

    public static IEnumerable<int> BaselineSizes => [10, 14];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 100, 300];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _paddedBySize = MemoizedSizes.ToDictionary(
            balloonCount => balloonCount,
            balloonCount => PaddedBalloons.FromNums(BurstBalloonsWorkloads.BuildBalloons(balloonCount, seed: RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int UnmemoizedRecursion(int balloonCount) =>
        BurstBalloonsSolution.MaxCoinsByUnmemoizedRecursion(_paddedBySize[balloonCount]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int balloonCount) =>
        BurstBalloonsSolution.MaxCoinsByMemoizedRecursion(_paddedBySize[balloonCount]);
}
