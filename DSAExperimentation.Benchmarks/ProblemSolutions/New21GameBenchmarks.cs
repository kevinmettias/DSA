using DSAExperimentation.LeetCode.New21Game;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are New21GameSolution's, the same methods New21GameSolutionTests
// proves correct. MaxPts is fixed at 6 and the stop point is the varying size (the
// limit equals it here, so only the reachability of the recursion tree matters,
// not the final probability value) so both the recursion depth (bounded by the stop
// point) and the branching factor (MaxPts) stay large enough for the un-memoized
// tree's overlapping-state blowup to show clearly.
//
// Sizes are per arm. The un-memoized tree stops at 20; the memoized arm runs on to
// 2,000, short of LC 837's 10^4 because Memoizer recurses once per running total and
// a deeper chain would put the call stack at risk. The two are compared at the sizes
// both run.
//
// There is no [GlobalSetup] left to charge: the only preparation the old harness did
// was copying the stop point into a separate field, and it is passed straight
// through to both arms.
public class New21GameBenchmarks
{
    private const int MaxPts = 6;

    public static IEnumerable<int> BaselineSizes => [12, 20];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 200, 2_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public double UnmemoizedRecursion(int stopAt) =>
        New21GameSolution.ProbabilityByUnmemoizedRecursion(stopAt, stopAt, MaxPts);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public double MemoizedRecursion(int stopAt) =>
        New21GameSolution.ProbabilityByMemoizedRecursion(stopAt, stopAt, MaxPts);
}
