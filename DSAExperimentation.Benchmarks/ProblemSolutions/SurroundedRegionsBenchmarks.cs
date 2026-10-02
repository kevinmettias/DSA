using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SurroundedRegions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SurroundedRegionsSolution's, the same methods
// SurroundedRegionsTests proves correct. Solve mutates the board it is
// handed - LeetCode's actual solve operation - so [IterationSetup] rebuilds
// a fresh random board before every iteration rather than reusing the one
// [GlobalSetup] built, which a single capture pass would leave stable. The two
// arms mark the border-reachable regions depth-first versus breadth-first and then
// share one flip pass, so the pair isolates that marking order.
[MemoryDiagnoser]
public class SurroundedRegionsBenchmarks
{
    private char[][] _board = [];

    [Params(50, 500)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup() => _board = SurroundedRegionsWorkloads.BuildBoard(Size, seed: 130);

    [IterationSetup]
    public void IterationSetup() => _board = SurroundedRegionsWorkloads.BuildBoard(Size, seed: 130);

    [Benchmark(Baseline = true)]
    public void BorderDepthFirstSearch() => SurroundedRegionsSolution.SolveByBorderDepthFirstSearch(_board);

    [Benchmark]
    public void BorderBreadthFirstSearch() => SurroundedRegionsSolution.SolveByBorderBreadthFirstSearch(_board);
}
