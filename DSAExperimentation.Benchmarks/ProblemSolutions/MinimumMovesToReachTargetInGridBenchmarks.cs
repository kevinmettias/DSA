using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumMovesToReachTargetInGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumMovesToReachTargetInGridSolution's, the same
// methods MinimumMovesToReachTargetInGridTests proves correct. MoveCount stays small
// enough that the bounded forward BFS baseline is still tractable at all - the whole
// point being that the composed backward reduction does not share that ceiling.
[MemoryDiagnoser]
public class MinimumMovesToReachTargetInGridBenchmarks
{
    private const int Seed = 3609;

    [Params(8, 14)]
    public int MoveCount;

    private (int Sx, int Sy, int Tx, int Ty) _pair;

    [GlobalSetup]
    public void Setup() => _pair = TargetGridWorkloads.BuildReachablePair(MoveCount, Seed);

    [Benchmark(Baseline = true)]
    public int BoundedForwardBfs() =>
        MinimumMovesToReachTargetInGridSolution.MinMovesByBoundedForwardBfs(_pair.Sx, _pair.Sy, _pair.Tx, _pair.Ty);

    [Benchmark]
    public int BackwardReduction() =>
        MinimumMovesToReachTargetInGridSolution.MinMovesByBackwardReduction(_pair.Sx, _pair.Sy, _pair.Tx, _pair.Ty);
}
