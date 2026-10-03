using DSAExperimentation.LeetCode.NQueens;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NQueensSolution's, the same methods NQueensSolutionTests
// proves correct. Both build LeetCode's actual answer shape - the list of boards -
// rather than only counting solutions as the pre-migration arms did.
public class NQueensBenchmarks
{
    // The search tree grows super-exponentially in Size, so the spread is multiplicative
    // rather than the arithmetic one most benchmarks here use: these three values are roughly
    // an order of magnitude apart in work, and 12 would put a single iteration into minutes.
    [Params(6, 8, 10)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public List<List<string>> RecursiveDfs() => NQueensSolution.SolveByRecursiveDfs(Size);

    [Benchmark]
    public List<List<string>> BacktrackEngine() => NQueensSolution.SolveByBacktrackEngine(Size);
}
