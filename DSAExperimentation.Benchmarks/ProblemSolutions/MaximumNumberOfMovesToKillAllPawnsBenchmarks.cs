using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumNumberOfMovesToKillAllPawns;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfMovesToKillAllPawnsSolution's, the
// same methods MaximumNumberOfMovesToKillAllPawnsTests proves correct. The
// composed arm is handed the prepared KnightDistances its hoisted overload takes,
// so every knight-distance BFS is charged to [GlobalSetup] rather than the
// minimax being measured.
[MemoryDiagnoser]
public class MaximumNumberOfMovesToKillAllPawnsBenchmarks
{
    private const int Seed = 3283;

    private int _kx;

    private int _ky;
    private int[][] _positions = [];
    private KnightDistances _knightDistances = null!;
    [Params(5, 15)]
    public int PawnCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        (_kx, _ky, _positions) = KnightPawnWorkloads.BuildGame(PawnCount, Seed);
        _knightDistances = MaximumNumberOfMovesToKillAllPawnsSolution.BuildKnightDistances(_kx, _ky, _positions);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceMinimax() =>
        MaximumNumberOfMovesToKillAllPawnsSolution.MaxMovesByBruteForceMinimax(_kx, _ky, _positions);

    [Benchmark]
    public int ReduceGraphMinimax() =>
        MaximumNumberOfMovesToKillAllPawnsSolution.MaxMovesByReduceGraphMinimax(_knightDistances);
}
