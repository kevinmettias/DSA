using DSAExperimentation.LeetCode.GuessNumberHigherOrLowerII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GuessNumberHigherOrLowerIISolution's, the same methods
// GuessNumberHigherOrLowerIISolutionTests proves correct. HighestNumber is the whole
// workload, so there is nothing to build in [GlobalSetup].
//
// Sizes are per arm. The un-memoized baseline's blowup is real, the same reasoning
// BurstBalloonsBenchmarks/FibonacciNumberBenchmarks already document, so it stops at
// 14; the memoized interval DP is O(n^3) and runs on to LC 375's own bound of 200, and
// the two are compared at the sizes both run.
public class GuessNumberHigherOrLowerIIBenchmarks
{
    public static IEnumerable<int> UnmemoizedSizes => [10, 14];

    public static IEnumerable<int> MemoizedSizes => [.. UnmemoizedSizes, 100, 200];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(UnmemoizedSizes))]
    public int UnmemoizedRecursion(int highestNumber) =>
        GuessNumberHigherOrLowerIISolution.GetMoneyAmountByUnmemoizedRecursion(highestNumber);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int highestNumber) =>
        GuessNumberHigherOrLowerIISolution.GetMoneyAmountByMemoizedRecursion(highestNumber);
}
