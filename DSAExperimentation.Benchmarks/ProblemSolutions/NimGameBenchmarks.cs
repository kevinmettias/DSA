using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NimGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NimGameSolution's, the same methods
// NimGameTests proves correct - the textbook O(n) memoized game-theory
// recursion vs. the closed-form O(1) n % 4 != 0 formula the recursion itself
// reduces to.
[MemoryDiagnoser]
public class NimGameBenchmarks
{
    [Params(20, 1_000)]
    public int Stones { get; set; }

    [Benchmark(Baseline = true)]
    public bool MemoizedRecursion() => NimGameSolution.CanWinByMemoizedRecursion(Stones);

    [Benchmark]
    public bool ModuloFormula() => NimGameSolution.CanWinByModuloFormula(Stones);
}
