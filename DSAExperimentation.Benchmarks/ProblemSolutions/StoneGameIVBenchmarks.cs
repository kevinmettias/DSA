using DSAExperimentation.LeetCode.StoneGameIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameIVSolution's, the same methods StoneGameIVSolutionTests
// proves correct. Plain un-memoized minimax recursion over the remaining stone count -
// exponential, since the same remaining count recurs through many different perfect-
// square-removal sequences reaching it - vs. the identical recurrence over this repo's
// own Memoizer caching that count, the shape DivisorGameBenchmarks and
// StoneGameIIIBenchmarks already use.
//
// Sizes are per arm. The un-memoized baseline's blowup is real (branching factor
// sqrt(stones), wider than DivisorGame's or StoneGameIII's own branching), so it stops
// at 20 stones; the memoized arm's O(n sqrt n) runs on to 2,000, short of LC 1510's
// 10^5 because Memoizer recurses once per remaining count and a deeper chain would put
// the call stack at risk. The two are compared at the sizes both run.
public class StoneGameIVBenchmarks
{
    public static IEnumerable<int> BaselineSizes => [16, 20];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 200, 2_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public bool CanAliceWinByUnmemoizedRecursion(int stoneCount) =>
        StoneGameIVSolution.CanAliceWinByUnmemoizedRecursion(stoneCount);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public bool CanAliceWinByMemoizedRecursion(int stoneCount) =>
        StoneGameIVSolution.CanAliceWinByMemoizedRecursion(stoneCount);
}
