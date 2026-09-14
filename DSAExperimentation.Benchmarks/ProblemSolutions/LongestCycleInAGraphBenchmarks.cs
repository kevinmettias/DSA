using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LongestCycleInAGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestCycleInAGraphSolution's, the same methods
// LongestCycleInAGraphTests proves correct. The naive per-start walk (no
// memoization across starts, so a shared cycle gets re-walked from every node that
// reaches it) is measured against this repo's own
// StronglyConnectedComponents.Tarjan, which finds every cycle in one O(V+E) pass.
// The workload is a single cycle spanning every node, so the naive walk pays its
// full O(n) re-walk from every one of the n starting points - a genuine O(n^2) vs
// O(n) split, not just a constant-factor difference.
//
// The Tarjan arm is handed a prepared FunctionalGraph so node construction is
// charged to [GlobalSetup] rather than to the search (#17.4); the walk arm takes
// LeetCode's own edges[] because that is already its input.
//
// NodeCount is capped well under Tarjan's real-recursion stack limit (StrongConnect/
// VisitChildren recurse two frames deep per graph edge - see StronglyConnectedComponents.cs's
// own doc comment on why this is real C# recursion, not an explicit stack): a single
// 3,000-node cycle already overflows the default 1 MB thread stack in this process, so
// this stays comfortably below that rather than pushing Tarjan past the depth it can
// safely handle.
[MemoryDiagnoser]
public class LongestCycleInAGraphBenchmarks
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
    public int PerStartWalk() => LongestCycleInAGraphSolution.LongestCycleByPerStartWalk(_edges);

    [Benchmark]
    public int TarjanScc() => LongestCycleInAGraphSolution.LongestCycleByTarjanComponents(_graph);
}
