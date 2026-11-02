using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.States;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.OpenTheLock;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OpenTheLockSolution's, the same methods
// OpenTheLockSolutionTests proves correct. Each arm is handed the prepared input its
// hoisted overload takes - a deadend Set for the mutation walk, a built lock graph
// for the Reduce.Graph walk - so graph construction is charged to [GlobalSetup]
// rather than to the search being measured.
public class OpenTheLockBenchmarks
{
    // LC problem number, reused as the deterministic deadend seed.
    private const int DeadendSeed = 752;

    private Set<string> _deadends = new();

    private StateGraph<string> _graph = null!;
    [Params(50, 500)]
    public int DeadendCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var deadends = LockWorkloads.BuildDeadends(DeadendCount, seed: DeadendSeed);
        _deadends = new Set<string>(deadends);
        _graph = OpenTheLockSolution.BuildLockGraph(deadends);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() =>
        OpenTheLockSolution.MinTurnsByMutationQueue(_deadends, LockScenario.FarthestTarget);

    [Benchmark]
    public int ReduceGraphBfs() =>
        OpenTheLockSolution.MinTurnsByReduceGraph(_graph, LockScenario.FarthestTarget);
}
