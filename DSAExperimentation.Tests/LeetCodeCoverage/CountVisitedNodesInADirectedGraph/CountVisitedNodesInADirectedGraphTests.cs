using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.CountVisitedNodesInADirectedGraph.Fixtures;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountVisitedNodesInADirectedGraph;

// LeetCode 2876. Count Visited Nodes in a Directed Graph: edges[i] gives each node's
// single outgoing edge, exactly the functional-graph shape LongestCycleInAGraphTests
// (LC 2360) already reduces onto this repo's own
// Algorithms.Connectivity.StronglyConnectedComponents.Tarjan - every SCC with more
// than one member is a simple cycle, and every node in it visits exactly
// component.Count distinct nodes before repeating.
//
// A node NOT on a cycle needs "1 + its successor's own answer" instead, which is a
// second, genuinely different composition: a multi-source BFS starting from every
// already-known cycle node and walking the REVERSED edges outward with this repo's
// own Queue<int> as the frontier - the same "peel a Queue<int> frontier layer by
// layer" idiom MinimumHeightTreesTests/TopologicalSort already establish, just seeded
// from known cycle answers instead of zero-in-degree leaves.
public sealed partial class CountVisitedNodesInADirectedGraphTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 0, 0], [3, 3, 3, 4] },
            { [1, 2, 3, 4, 0], [5, 5, 5, 5, 5] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountVisitedNodes_LeetCodeExamples_ReturnsExpectedVisitCounts(int[] edges, int[] expected)
    {
        var visited = CountVisitedNodes(edges);

        Assert.Equal(expected, visited);
    }

    private static int[] CountVisitedNodes(int[] edges)
    {
        var n = edges.Length;
        var nodes = BuildFunctionalGraph(edges);

        var components = StronglyConnectedComponents.Tarjan<
            FunctionalGraphNode, FunctionalGraphTopology, ListChildren<FunctionalGraphNode>,
            NaturalChildOrder<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>, ListChildren<FunctionalGraphNode>>(nodes);

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

        var predecessors = BuildPredecessors(edges);

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

    private static List<int>[] BuildPredecessors(int[] edges)
    {
        var predecessors = new List<int>[edges.Length];
        for (var i = 0; i < edges.Length; i++)
        {
            predecessors[i] = [];
        }

        for (var i = 0; i < edges.Length; i++)
        {
            predecessors[edges[i]].Add(i);
        }

        return predecessors;
    }

    private static FunctionalGraphNode[] BuildFunctionalGraph(int[] edges)
    {
        var nodes = new FunctionalGraphNode[edges.Length];
        for (var i = 0; i < edges.Length; i++)
        {
            nodes[i] = new FunctionalGraphNode(i);
        }

        for (var i = 0; i < edges.Length; i++)
        {
            nodes[i].Successors.Add(nodes[edges[i]]);
        }

        return nodes;
    }
}
