using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NQueens;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NQueensSolution's, the same methods NQueensTests
// proves correct. Both build LeetCode's actual answer shape - the list of boards -
// rather than only counting solutions as the pre-migration arms did.
[MemoryDiagnoser]
public class NQueensBenchmarks
{
    [Params(8)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public List<List<string>> RecursiveDfs() => NQueensSolution.SolveByRecursiveDfs(Size);

    [Benchmark]
    public List<List<string>> BacktrackEngine() => NQueensSolution.SolveByBacktrackEngine(Size);
}
