using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SpiralMatrixII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpiralMatrixIISolution's, the same methods
// SpiralMatrixIITests proves correct. Both visit exactly Size^2 cells - the gap is
// per-cell overhead, not algorithm class.
[MemoryDiagnoser]
public class SpiralMatrixIIBenchmarks
{
    [Params(10, 100)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public int[][] DirectionVectorWithVisitedSet() => SpiralMatrixIISolution.GenerateMatrixByDirectionVectorWalk(Size);

    [Benchmark]
    public int[][] BoundaryShrinking() => SpiralMatrixIISolution.GenerateMatrixByBoundaryShrinking(Size);
}
