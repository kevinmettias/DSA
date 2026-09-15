using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NQueensII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NQueensIISolution's, the same methods
// NQueensIITests proves correct.
[MemoryDiagnoser]
public class NQueensIIBenchmarks
{
    [Params(8)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public int ArrayRecursion() => NQueensIISolution.TotalNQueensByArrayRecursion(Size);

    [Benchmark]
    public int BacktrackSearch() => NQueensIISolution.TotalNQueensByBacktrackSearch(Size);
}
