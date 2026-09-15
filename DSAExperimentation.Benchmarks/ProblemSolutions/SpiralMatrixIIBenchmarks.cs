using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SpiralMatrixII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpiralMatrixIISolution's, the same methods
// SpiralMatrixIITests proves correct. Both visit exactly n^2 cells - the gap is
// per-cell overhead, not algorithm class.
[MemoryDiagnoser]
public class SpiralMatrixIIBenchmarks
{
    [Params(10, 100)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public int[][] DirectionVectorWithVisitedSet() => SpiralMatrixIISolution.GenerateMatrixByDirectionVectorWalk(N);

    [Benchmark]
    public int[][] BoundaryShrinking() => SpiralMatrixIISolution.GenerateMatrixByBoundaryShrinking(N);
}
