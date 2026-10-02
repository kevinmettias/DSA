using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfIslands;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfIslandsSolution's, the same methods
// NumberOfIslandsTests proves correct. Roughly half land, half water keeps the
// flood fill busy across many separate islands rather than one solid block; the
// solution's own claimed-set tracking (not grid mutation) is what lets the same
// grid be reused, unchanged, across every invocation. The two arms differ only in
// the order their flood fill drains a component's frontier - depth-first versus
// breadth-first - so the pair isolates that order.
[MemoryDiagnoser]
public class NumberOfIslandsBenchmarks
{
    private const int Seed = 200; private char[][] _grid = [];

    // LC problem number

    [Params(50, 500)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed);

    [Benchmark(Baseline = true)]
    public int DepthFirstSink() => NumberOfIslandsSolution.CountIslandsByDepthFirstSink(_grid);

    [Benchmark]
    public int BreadthFirstSink() => NumberOfIslandsSolution.CountIslandsByBreadthFirstSink(_grid);
}
