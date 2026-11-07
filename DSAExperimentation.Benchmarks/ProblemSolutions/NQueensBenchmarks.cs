using DSAExperimentation.LeetCode.Solutions.NQueens;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: every arm is NQueensSolution's, the same methods NQueensSolutionTests
// proves correct. Each builds LeetCode's actual answer shape - the list of boards -
// rather than only counting solutions as the pre-migration arms did. The exact-cover
// arm lists the boards in another order, which LC 51 allows, and builds its dancing
// links on every call.
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

    [Benchmark]
    public List<List<string>> ExactCover() => NQueensSolution.SolveByExactCover(Size);
}
