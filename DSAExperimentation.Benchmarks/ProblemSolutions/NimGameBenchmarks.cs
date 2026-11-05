using DSAExperimentation.LeetCode.Solutions.NimGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NimGameSolution's, the same methods
// CatalogCaseTests proves correct - the textbook O(n) memoized game-theory
// recursion vs. the closed-form O(1) n % 4 != 0 formula the recursion itself
// reduces to.
//
// The two sizes answer both ways, so an arm that gave one verdict every time could not
// agree at both: 21 stones is a win for the mover, and 1,000, a multiple of 4, a loss.
public class NimGameBenchmarks
{
    [Params(21, 1_000)]
    public int Stones { get; set; }

    [Benchmark]
    public bool CanWinByMemoizedRecursion() => NimGameSolution.CanWinByMemoizedRecursion(Stones);

    [Benchmark(Baseline = true)]
    public bool CanWinByModuloFormula() => NimGameSolution.CanWinByModuloFormula(Stones);
}
