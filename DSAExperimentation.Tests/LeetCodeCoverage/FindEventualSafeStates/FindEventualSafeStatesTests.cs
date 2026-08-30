using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.FindEventualSafeStates.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindEventualSafeStates;

// LeetCode 802. Find Eventual Safe States: a node is safe iff every path out of it
// eventually reaches a terminal node (no path loops forever). Reframed as Kahn's
// algorithm over the REVERSED graph - each node's "children" are its predecessors -
// seeded from terminal nodes (out-degree zero, i.e. reversed in-degree zero) and
// peeled backward: this repo's own TopologicalSort.TrySort already IS that peel.
// Its `ordering` out-parameter is exactly the safe nodes regardless of TrySort's own
// bool result (which only says whether every node was safe, i.e. the graph was
// acyclic) - the leftover, never-peeled nodes are exactly the unsafe ones, the same
// "cycle announces itself as leftover in-degree" property TrySort's own doc comment
// already names.
public sealed partial class FindEventualSafeStatesTests
{
    [Fact]
    public void EventualSafeNodes_MixOfChainsAndACycle_ReturnsSortedSafeNodeIds()
    {
        int[][] graph = [[1, 2], [2, 3], [5], [0], [5], [], []];

        var safe = EventualSafeNodes(graph);

        Assert.Equal([2, 4, 5, 6], safe);
    }

    [Fact]
    public void EventualSafeNodes_SelfLoopAndOnlyOneTerminalNode_ReturnsThatNodeAlone()
    {
        int[][] graph = [[1, 2, 3, 4], [1, 2], [3, 4], [0, 4], []];

        var safe = EventualSafeNodes(graph);

        Assert.Equal([4], safe);
    }

    private static int[] EventualSafeNodes(int[][] graph)
    {
        var nodes = new SafeStateNode[graph.Length];

        for (var i = 0; i < graph.Length; i++)
        {
            nodes[i] = new SafeStateNode(i);
        }

        for (var i = 0; i < graph.Length; i++)
        {
            foreach (var next in graph[i])
            {
                nodes[next].Predecessors.Add(nodes[i]);
            }
        }

        TopologicalSort.TrySort<
            SafeStateNode, SafeStateTopology, ListChildren<SafeStateNode>,
            NaturalChildOrder<SafeStateNode, ListChildren<SafeStateNode>>, ListChildren<SafeStateNode>>(
            nodes, out var ordering);

        return ordering.Select(node => node.Id).OrderBy(id => id).ToArray();
    }
}
