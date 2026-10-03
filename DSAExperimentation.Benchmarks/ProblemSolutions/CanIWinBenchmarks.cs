using DSAExperimentation.LeetCode.CanIWin;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CanIWinSolution's, the same methods CanIWinSolutionTests
// proves correct. desiredTotal is set to the exact sum of every choosable number,
// so a win can only be confirmed on the very last pick - forcing BOTH strategies
// through their full worst-case search tree instead of an early exit on the first
// invocation making the brute force look artificially fast (the same "force the
// real worst case" convention TwoSumBenchmarks already uses).
//
// Sizes are per arm. The brute force re-explores every pick order, O(n!), so it stops
// at 8 choosable numbers; the memoized arm solves each of the 2^n used-number masks
// once and runs on to LC 464's own bound of 20. The two are compared at the sizes both
// run.
public class CanIWinBenchmarks
{
    private const int GaussSumDivisor = 2;

    private Dictionary<int, int> _desiredTotalByMaxChoosable = [];

    public static IEnumerable<int> BruteForceSizes => [6, 8];

    public static IEnumerable<int> MemoizedSizes => [.. BruteForceSizes, 14, 20];

    // Every size any arm runs has its desired total derived here, outside the timed region;
    // an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _desiredTotalByMaxChoosable = MemoizedSizes.ToDictionary(
            maxChoosable => maxChoosable,
            maxChoosable => maxChoosable * (maxChoosable + 1) / GaussSumDivisor);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public bool CanWinByBruteForceRecursion(int maxChoosableInteger) =>
        CanIWinSolution.CanWinByBruteForceRecursion(maxChoosableInteger, _desiredTotalByMaxChoosable[maxChoosableInteger]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public bool CanWinByMemoizedRecursion(int maxChoosableInteger) =>
        CanIWinSolution.CanWinByMemoizedRecursion(maxChoosableInteger, _desiredTotalByMaxChoosable[maxChoosableInteger]);
}
