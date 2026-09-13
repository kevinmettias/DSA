using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ProjectionAreaOf3DShapes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are ProjectionAreaOf3DShapesSolution's, the same
// methods ProjectionAreaOf3DShapesTests proves correct. Every strategy takes
// LeetCode's own int[][] grid, which [GlobalSetup] already builds, so no hoisted
// overload is needed - grid construction is never charged to a measured call. All
// three are O(rows * cols), so what this isolates is redundant-pass overhead rather
// than an algorithm-class swap.
[MemoryDiagnoser]
public class ProjectionAreaOf3DShapesBenchmarks
{
    private const int CellHeightBound = 100;

    private const int RandomSeed = 1;

    [Params(50, 500)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, CellHeightBound)).ToArray())
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
