using DSAExperimentation.LeetCode.Solutions.NimGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NimGameSolution's, the same methods
// CatalogCaseTests proves correct - the textbook O(n) memoized game-theory
// recursion vs. the closed-form O(1) n % 4 != 0 formula the recursion itself
// reduces to.
public class NimGameBenchmarks
{
    [Params(20, 1_000)]
    public int Stones { get; set; }

    [Benchmark]
    public bool CanWinByMemoizedRecursion() => NimGameSolution.CanWinByMemoizedRecursion(Stones);

    [Benchmark(Baseline = true)]
    public bool CanWinByModuloFormula() => NimGameSolution.CanWinByModuloFormula(Stones);
}
