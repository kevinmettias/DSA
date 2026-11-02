using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfIslands;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfIslandsSolution's, the same methods
// NumberOfIslandsSolutionTests proves correct. Roughly half land, half water keeps the
// flood fill busy across many separate islands rather than one solid block; the
// solution's own claimed-set tracking (not grid mutation) is what lets the same
// grid be reused, unchanged, across every invocation. The baseline is a hand-written
// breadth-first flood fill; the other arm is ConnectedComponents over GridTopology, so
// the pair measures what the engine's walk costs against the loop it stands for.
// GridSize stops at LC 200's own bound of 300.
public class NumberOfIslandsBenchmarks
{
    private const int Seed = 200; private char[][] _grid = [];

    // LC problem number

    [Params(50, 300)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed);

    [Benchmark]
    public int ConnectedComponents() => NumberOfIslandsSolution.CountIslandsByConnectedComponents(_grid);

    [Benchmark(Baseline = true)]
    public int BreadthFirstSink() => NumberOfIslandsSolution.CountIslandsByBreadthFirstSink(_grid);
}
