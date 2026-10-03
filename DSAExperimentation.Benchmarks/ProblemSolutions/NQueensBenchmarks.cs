using DSAExperimentation.LeetCode.NQueens;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NQueensSolution's, the same methods NQueensSolutionTests
// proves correct. Both build LeetCode's actual answer shape - the list of boards -
// rather than only counting solutions as the pre-migration arms did.
public class NQueensBenchmarks
{
    // The search tree grows super-exponentially in Size, so even these close sizes are far
    // apart in work. Size stops at LC 51's own bound of 9.
    [Params(6, 8, 9)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public List<List<string>> RecursiveDfs() => NQueensSolution.SolveByRecursiveDfs(Size);

    [Benchmark]
    public List<List<string>> BacktrackEngine() => NQueensSolution.SolveByBacktrackEngine(Size);
}
