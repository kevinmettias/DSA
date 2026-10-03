using DSAExperimentation.LeetCode.SpiralMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpiralMatrixSolution's, the same methods
// SpiralMatrixSolutionTests proves correct - a visited-grid simulation
// (O(rows*cols) extra memory for the visited flags) vs. the
// four-boundary-pointer shrink (O(1) extra memory, no visited tracking at
// all). Size stops at LC 54's 10 x 10, whose cells 0..99 stay inside its
// [-100, 100].
public class SpiralMatrixBenchmarks
{
    private int[][] _matrix = [];

    [Params(3, 10)]
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
