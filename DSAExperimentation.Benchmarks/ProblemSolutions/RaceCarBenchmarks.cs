using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RaceCar;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RaceCarSolution's, the same methods RaceCarTests
// proves correct - the textbook mutate-the-tuple BFS (Queue<(int,int)> plus a
// HashSet<(int,int)> visited set, generating each of the two candidate commands on
// the fly) against this repo's own BFS, Reduce.Graph + DistanceMapReduceAlgebra
// over a precomputed RaceCarNode graph, the same composition OpenTheLockBenchmarks
// uses for LC 752. Both explore the same pre-bounded (position, speed) state space,
// and each is handed the prepared input its hoisted overload takes - the bound
// alone for the naive arm, the materialized graph for the composed one - so
// construction is charged to [GlobalSetup] rather than to the search being
// measured.
[MemoryDiagnoser]
public class RaceCarBenchmarks
{
    private RaceCarStateSpace _space;

    private RaceCarStateGraph _graph = null!;
    [Params(6, 25)]
    public int Target { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _space = RaceCarStateSpace.For(Target);
        _graph = RaceCarStateGraph.Build(Target);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() => RaceCarSolution.MinCommandsByMutationQueue(_space, Target);

    [Benchmark]
    public int ReduceGraphBfs() => RaceCarSolution.MinCommandsByReduceGraph(_graph, Target);
}
