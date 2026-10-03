using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ProjectionAreaOf3DShapes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are ProjectionAreaOf3DShapesSolution's, the same
// methods ProjectionAreaOf3DShapesSolutionTests proves correct. Every strategy takes
// LeetCode's own int[][] grid, which [GlobalSetup] already builds, so no hoisted
// overload is needed - grid construction is never charged to a measured call. All
// three are O(rows * cols), so what this isolates is redundant-pass overhead rather
// than an algorithm-class swap. Size stops at LC 883's 50 x 50 grid, and every cell
// height is drawn from its [0, 50].
public class ProjectionAreaOf3DShapesBenchmarks
{
    private const int CellHeightBound = 51;

    private const int RandomSeed = 1;

    private int[][] _grid = [];

    [Params(10, 50)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => SeededDraws.Values(Size, 0, CellHeightBound, random))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ThreeSeparatePasses() =>
        ProjectionAreaOf3DShapesSolution.ProjectionAreaByThreeSeparatePasses(_grid);

    [Benchmark]
    public int RowAndColumnPasses() =>
        ProjectionAreaOf3DShapesSolution.ProjectionAreaByRowAndColumnPasses(_grid);

    [Benchmark]
    public int SingleCombinedPass() =>
        ProjectionAreaOf3DShapesSolution.ProjectionAreaBySingleCombinedPass(_grid);
}
