using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CanIWin;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CanIWinSolution's, the same methods CanIWinTests
// proves correct. desiredTotal is set to the exact sum of every choosable number,
// so a win can only be confirmed on the very last pick - forcing BOTH strategies
// through their full worst-case search tree instead of an early exit on the first
// invocation making the brute force look artificially fast (the same "force the
// real worst case" convention TwoSumBenchmarks already uses).
[MemoryDiagnoser]
public class CanIWinBenchmarks
{
    private const int GaussSumDivisor = 2;

    private int _desiredTotal;

    [Params(6, 8)]
    public int MaxChoosableInteger { get; set; }

    [GlobalSetup]
    public void Setup() => _desiredTotal = MaxChoosableInteger * (MaxChoosableInteger + 1) / GaussSumDivisor;

    [Benchmark(Baseline = true)]
    public bool BruteForceRecursion() => CanIWinSolution.CanWinByBruteForceRecursion(MaxChoosableInteger, _desiredTotal);

    [Benchmark]
    public bool MemoizedRecursion() => CanIWinSolution.CanWinByMemoizedRecursion(MaxChoosableInteger, _desiredTotal);
}
