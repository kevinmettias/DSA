using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfIslands;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is NumberOfIslandsSolution's, the same method
// NumberOfIslandsTests proves correct. Roughly half land, half water keeps the
// flood fill busy across many separate islands rather than one solid block; the
// solution's own claimed-set tracking (not grid mutation) is what lets the same
// grid be reused, unchanged, across every invocation.
[MemoryDiagnoser]
public class NumberOfIslandsBenchmarks
{
    private const int Seed = 200; private char[][] _grid = [];

    // LC problem number

    [Params(50, 500)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed);

    [Benchmark]
    public int DepthFirstSink() => NumberOfIslandsSolution.CountIslandsByDepthFirstSink(_grid);
}
