using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.Domain.Locks;
using DSAExperimentation.LeetCode.OpenTheLock;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OpenTheLockSolution's, the same methods
// OpenTheLockTests proves correct. Each arm is handed the prepared input its
// hoisted overload takes - a deadend Set for the mutation walk, a built LockGraph
// for the Reduce.Graph walk - so graph construction is charged to [GlobalSetup]
// rather than to the search being measured.
[MemoryDiagnoser]
public class OpenTheLockBenchmarks
{
    // LC problem number, reused as the deterministic deadend seed.
    private const int DeadendSeed = 752;

    [Params(0, 500)]
    public int DeadendCount;

    private Set<string> _deadends = null!;
    private LockGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        var deadends = LockWorkloads.BuildDeadends(DeadendCount, seed: DeadendSeed);
        _deadends = new Set<string>(deadends);
        _graph = LockGraph.Build(deadends);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() =>
        OpenTheLockSolution.MinTurnsByMutationQueue(_deadends, LockWorkloads.FarthestTarget);

    [Benchmark]
    public int ReduceGraphBfs() =>
        OpenTheLockSolution.MinTurnsByReduceGraph(_graph, LockWorkloads.FarthestTarget);
}
