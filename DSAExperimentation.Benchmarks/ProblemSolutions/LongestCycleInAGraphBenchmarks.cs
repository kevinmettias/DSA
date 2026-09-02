using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Cycle in a Graph (LC 2360): the naive per-start-node walk (no memoization
// across starts, so a shared cycle gets re-walked from every node inside it) vs. this
// repo's own Algorithms.Connectivity.StronglyConnectedComponents.Tarjan, which finds
// every cycle in one O(V+E) pass. Input is a single cycle spanning every node, so the
// naive walk pays its full O(n) re-walk cost from every one of the n starting points -
// a genuine O(n^2) vs. O(n) split, not just a constant-factor difference.
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
    private FunctionalGraphNode[] _nodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _edges = FunctionalGraphs.BuildSingleCycleEdges(NodeCount);
        _nodes = FunctionalGraphs.BuildNodes(_edges);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var longest = -1;

        for (var start = 0; start < _edges.Length; start++)
        {
            longest = Math.Max(longest, WalkFrom(start));
        }

        return longest;
    }

    private int WalkFrom(int start)
    {
        var stepOf = new Dictionary<int, int>();
        var current = start;
        var step = 0;

        while (current != -1 && !stepOf.ContainsKey(current))
        {
            stepOf[current] = step++;
            current = _edges[current];
        }

        return current == -1 ? -1 : step - stepOf[current];
    }

    [Benchmark]
    public int TarjanSCC()
    {
        var components = StronglyConnectedComponents.Tarjan<
            FunctionalGraphNode, FunctionalGraphTopology, ListChildren<FunctionalGraphNode>,
            NaturalChildOrder<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>, ListChildren<FunctionalGraphNode>>(_nodes);

        var longest = -1;

        foreach (var component in components)
        {
            if (component.Count > 1 && component.Count > longest)
            {
                longest = component.Count;
            }
        }

        return longest;
    }
}
