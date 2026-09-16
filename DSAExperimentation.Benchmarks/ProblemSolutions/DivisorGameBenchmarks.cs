using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DivisorGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DivisorGameSolution's, the same methods DivisorGameTests
// proves correct. The O(n^2) memoized game-theory recursion scanning every divisor of
// the position vs. the closed-form O(1) "an even position wins" formula it reduces to.
[MemoryDiagnoser]
public class DivisorGameBenchmarks
{
    [Params(100, 1_000)]
    public int StartingPosition { get; set; }

    [Benchmark(Baseline = true)]
    public bool CanAliceWinByMemoizedRecursion() =>
        DivisorGameSolution.CanAliceWinByMemoizedRecursion(StartingPosition);

    [Benchmark]
    public bool CanAliceWinByParityFormula() =>
        DivisorGameSolution.CanAliceWinByParityFormula(StartingPosition);
}
