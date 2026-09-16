using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReachingPoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReachingPointsSolution's, the same methods
// ReachingPointsTests proves correct. TargetY is fixed small so growing TargetX makes
// the subtractive reduction's step count grow with it, while the modulo reduction's
// stays flat.
[MemoryDiagnoser]
public class ReachingPointsBenchmarks
{
    private const int SourceX = 1;
    private const int SourceY = 1;
    private const int TargetY = 3;

    [Params(10_000, 10_000_000)]
    public int TargetX { get; set; }

    [Benchmark(Baseline = true)]
    public bool IsReachableBySubtractiveReduction() =>
        ReachingPointsSolution.IsReachableBySubtractiveReduction(SourceX, SourceY, TargetX, TargetY);

    [Benchmark]
    public bool IsReachableByModuloReduction() =>
        ReachingPointsSolution.IsReachableByModuloReduction(SourceX, SourceY, TargetX, TargetY);
}
