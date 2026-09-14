using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountVisitedNodesInADirectedGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountVisitedNodesInADirectedGraphSolution's, the same
// methods CountVisitedNodesInADirectedGraphTests proves correct. The workload is
// the single-cycle-spanning-every-node shape LongestCycleInAGraphBenchmarks already
// builds via FunctionalGraphs.BuildSingleCycleEdges - every start node's naive
// forward walk has to traverse the whole cycle before it repeats a node, so nothing
// short-circuits early (a genuine O(n^2) vs O(n) split, not just a constant-factor
// difference).
//
// The composed arm is handed a prepared FunctionalGraph so node construction is
// charged to [GlobalSetup] rather than to the search (#17.4); the walk arm takes
// LeetCode's own edges[] because that is already its input.
//
// NodeCount stays at or under 2,000 for the same reason LongestCycleInAGraph
// Benchmarks caps there: Tarjan recurses with real C# call frames, and a single
// cycle much longer than that overflows the default 1 MB thread stack.
[MemoryDiagnoser]
public class CountVisitedNodesInADirectedGraphBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private int[] _edges = null!;
    private FunctionalGraph _graph;

    [GlobalSetup]
    public void Setup()
    {
        _edges = FunctionalGraphs.BuildSingleCycleEdges(NodeCount);
        _graph = FunctionalGraph.Build(_edges);
    }

    [Benchmark(Baseline = true)]
    public int[] PerStartWalk() =>
        CountVisitedNodesInADirectedGraphSolution.CountVisitedNodesByPerStartWalk(_edges);

    [Benchmark]
    public int[] SccPlusReverseBfs() =>
        CountVisitedNodesInADirectedGraphSolution.CountVisitedNodesBySccAndReverseBfs(_graph);
}
