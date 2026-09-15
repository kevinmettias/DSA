using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DivisorGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DivisorGameSolution's, the same methods DivisorGameTests
// proves correct. The O(n^2) memoized game-theory recursion scanning every divisor of
// n vs. the closed-form O(1) "n is even" formula the recursion reduces to.
[MemoryDiagnoser]
public class DivisorGameBenchmarks
{
    [Params(100, 1_000)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public bool MemoizedRecursion() => DivisorGameSolution.AliceWinsByMemoizedRecursion(N);

    [Benchmark]
    public bool ParityFormula() => DivisorGameSolution.AliceWinsByParityFormula(N);
}
