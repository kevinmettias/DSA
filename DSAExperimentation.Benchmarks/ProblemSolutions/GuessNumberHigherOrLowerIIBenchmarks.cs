using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GuessNumberHigherOrLowerII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GuessNumberHigherOrLowerIISolution's, the same methods
// GuessNumberHigherOrLowerIITests proves correct. HighestNumber is kept modest
// specifically because the un-memoized baseline's blowup is real, the same reasoning
// BurstBalloonsBenchmarks/FibonacciBenchmarks already document.
[MemoryDiagnoser]
public class GuessNumberHigherOrLowerIIBenchmarks
{
    [Params(10, 14)]
    public int HighestNumber { get; set; }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        GuessNumberHigherOrLowerIISolution.GetMoneyAmountByUnmemoizedRecursion(HighestNumber);

    [Benchmark]
    public int MemoizedRecursion() =>
        GuessNumberHigherOrLowerIISolution.GetMoneyAmountByMemoizedRecursion(HighestNumber);
}
