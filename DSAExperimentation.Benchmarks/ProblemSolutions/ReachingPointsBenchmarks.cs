using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReachingPoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReachingPointsSolution's, the same methods
// ReachingPointsTests proves correct. Ty is fixed small so growing Tx makes the
// subtractive reduction's step count grow with it, while the modulo reduction's stays
// flat.
[MemoryDiagnoser]
public class ReachingPointsBenchmarks
{
    private const int Sx = 1;
    private const int Sy = 1;
    private const int Ty = 3;

    [Params(10_000, 10_000_000)]
    public int Tx { get; set; }

    [Benchmark(Baseline = true)]
    public bool SubtractiveBackwardReduction() =>
        ReachingPointsSolution.IsReachableBySubtractiveReduction(Sx, Sy, Tx, Ty);

    [Benchmark]
    public bool ModuloBackwardReduction() =>
        ReachingPointsSolution.IsReachableByModuloReduction(Sx, Sy, Tx, Ty);
}
