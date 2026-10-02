using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NQueensII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NQueensIISolution's, the same methods
// NQueensIITests proves correct.
[MemoryDiagnoser]
public class NQueensIIBenchmarks
{
    // Counting only, with no board ever materialised, so this arm can afford two sizes past
    // where NQueensBenchmarks stops. Super-exponential in Size all the same - 14 is minutes.
    [Params(8, 10, 12)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public int ArrayRecursion() => NQueensIISolution.TotalNQueensByArrayRecursion(Size);

    [Benchmark]
    public int BacktrackSearch() => NQueensIISolution.TotalNQueensByBacktrackSearch(Size);
}
