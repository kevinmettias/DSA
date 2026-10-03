using DSAExperimentation.LeetCode.NQueensII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NQueensIISolution's, the same methods
// NQueensIISolutionTests proves correct.
public class NQueensIIBenchmarks
{
    // Counting only, with no board ever materialised, but still super-exponential in Size.
    // Size stops at LC 52's own bound of 9, where NQueensBenchmarks also stops.
    [Params(8, 9)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public int ArrayRecursion() => NQueensIISolution.TotalNQueensByArrayRecursion(Size);

    [Benchmark]
    public int BacktrackSearch() => NQueensIISolution.TotalNQueensByBacktrackSearch(Size);
}
