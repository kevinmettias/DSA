using DSAExperimentation.LeetCode.StoneGameIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameIVSolution's, the same methods StoneGameIVSolutionTests
// proves correct. Plain un-memoized minimax recursion over the remaining stone count -
// exponential, since the same remaining count recurs through many different perfect-
// square-removal sequences reaching it - vs. the same recurrence settled bottom-up in one
// ascending pass over a table of every count, where each losing count marks the counts one
// square above it as wins.
//
// Sizes are per arm. The un-memoized baseline's blowup is real (branching factor
// sqrt(stones), wider than DivisorGame's or StoneGameIII's own branching), so it stops
// at 20 stones; the table runs on to LC 1510's own bound of 10^5 - it never recurses, so
// nothing but its O(n sqrt n) time grows with n. The two are compared at the sizes both run.
public class StoneGameIVBenchmarks
{
    public static IEnumerable<int> BaselineSizes => [16, 20];

    public static IEnumerable<int> TableSizes => [.. BaselineSizes, 1_000, 100_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public bool CanAliceWinByUnmemoizedRecursion(int stoneCount) =>
        StoneGameIVSolution.CanAliceWinByUnmemoizedRecursion(stoneCount);

    [Benchmark]
    [ArgumentsSource(nameof(TableSizes))]
    public bool CanAliceWinByBottomUpTable(int stoneCount) =>
        StoneGameIVSolution.CanAliceWinByBottomUpTable(stoneCount);
}
