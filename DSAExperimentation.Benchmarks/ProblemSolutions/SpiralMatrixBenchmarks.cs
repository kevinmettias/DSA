using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SpiralMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpiralMatrixSolution's, the same methods
// SpiralMatrixTests proves correct - a visited-grid simulation
// (O(rows*cols) extra memory for the visited flags) vs. the
// four-boundary-pointer shrink (O(1) extra memory, no visited tracking at
// all).
[MemoryDiagnoser]
public class SpiralMatrixBenchmarks
{
    private int[][] _matrix = [];

    [Params(20, 100)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var value = 0;
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => value++).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public IList<int> VisitedGridWalk() => SpiralMatrixSolution.SpiralOrderByVisitedGridWalk(_matrix);

    [Benchmark]
    public IList<int> BoundaryPointerShrink() => SpiralMatrixSolution.SpiralOrderByBoundaryPointerShrink(_matrix);
}
