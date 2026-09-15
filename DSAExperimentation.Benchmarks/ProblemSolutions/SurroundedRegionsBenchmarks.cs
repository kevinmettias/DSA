using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SurroundedRegions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is SurroundedRegionsSolution's, the same method
// SurroundedRegionsTests proves correct. Solve mutates the board it is
// handed - LeetCode's actual solve operation - so [IterationSetup] rebuilds
// a fresh random board before every iteration rather than reusing the one
// [GlobalSetup] built, which a single capture pass would leave stable.
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

    [Benchmark]
    public void BorderDepthFirstSearch() => SurroundedRegionsSolution.SolveByBorderDepthFirstSearch(_board);
}
