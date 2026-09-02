using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Visited Nodes in a Directed Graph (LC 2876): same functional-graph shape and
// the same single-cycle-spanning-every-node worst case LongestCycleInAGraphBenchmarks
// already builds via FunctionalGraphs.BuildSingleCycleEdges - every start node's naive
// forward walk has to traverse the whole cycle before it repeats a node, so nothing
// short-circuits early (a genuine O(n^2) vs O(n) split, not just a constant-factor
// difference). The primitive-based arm finds every cycle in one O(V+E)
// StronglyConnectedComponents.Tarjan pass, then propagates "1 + successor's own
// answer" outward from cycle nodes via one more O(V+E) multi-source Queue<int> BFS
// over the reversed edges. NodeCount stays at or under 2,000 for the same reason
// LongestCycleInAGraphBenchmarks caps there: Tarjan recurses with real C# call
// frames, and a single cycle much longer than that overflows the default 1 MB thread
// stack - not something this composing-only task should work around.
[MemoryDiagnoser]
public class CountVisitedNodesInADirectedGraphBenchmarks
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
    public int[] BruteForce()
    {
        var answer = new int[_edges.Length];

        for (var start = 0; start < _edges.Length; start++)
        {
            answer[start] = WalkFrom(start);
        }

        return answer;
    }

    private int WalkFrom(int start)
    {
        var visited = new HashSet<int>();
        var current = start;

        while (visited.Add(current))
        {
            current = _edges[current];
        }

        return visited.Count;
    }

    [Benchmark]
    public int[] SccPlusReverseBfs()
    {
        var components = StronglyConnectedComponents.Tarjan<
            FunctionalGraphNode, FunctionalGraphTopology, ListChildren<FunctionalGraphNode>,
            NaturalChildOrder<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>, ListChildren<FunctionalGraphNode>>(_nodes);

        var n = _edges.Length;
        var answer = new int[n];
        var settled = new bool[n];
        var frontier = new RepoQueue();

        foreach (var component in components)
        {
            if (component.Count <= 1)
            {
                continue;
            }

            foreach (var node in component)
            {
                answer[node.Id] = component.Count;
                settled[node.Id] = true;
                frontier.Enqueue(node.Id);
            }
        }

        var predecessors = BuildPredecessors(n);

        while (frontier.TryDequeue(out var current))
        {
            foreach (var predecessor in predecessors[current])
            {
                if (settled[predecessor])
                {
                    continue;
                }

                answer[predecessor] = answer[current] + 1;
                settled[predecessor] = true;
                frontier.Enqueue(predecessor);
            }
        }

        return answer;
    }

    private List<int>[] BuildPredecessors(int n)
    {
        var predecessors = new List<int>[n];
        for (var i = 0; i < n; i++)
        {
            predecessors[i] = [];
        }

        for (var i = 0; i < n; i++)
        {
            predecessors[_edges[i]].Add(i);
        }

        return predecessors;
    }
}
