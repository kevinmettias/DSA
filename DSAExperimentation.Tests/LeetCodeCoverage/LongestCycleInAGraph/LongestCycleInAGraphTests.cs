using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.LongestCycleInAGraph.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCycleInAGraph;

// LeetCode 2360. Longest Cycle in a Graph: edges[i] gives each node's single outgoing
// edge (or -1 for none), so this is exactly a functional graph - every node has out-
// degree at most 1. That constraint is what makes this problem reduce cleanly onto
// this repo's own Algorithms.Connectivity.StronglyConnectedComponents.Tarjan: with at
// most one outgoing edge per node, a strongly connected component can never contain
// more structure than a single simple cycle, so the answer is just the largest
// component with more than one member (or -1 if every component is a singleton, i.e.
// no cycle exists at all). One O(V+E) Tarjan pass over an IGraphTopology built from
// the edges array, no bespoke cycle-detection logic needed.
public sealed partial class LongestCycleInAGraphTests
{
    [Fact]
    public void LongestCycle_TailFeedsIntoAThreeNodeCycle_ReturnsCycleLength()
    {
        // 0 -> 3, 1 -> 3, 2 -> 4 -> 3 -> 2 (cycle {2, 3, 4}, length 3).
        int[] edges = [3, 3, 4, 2, 3];

        var longest = LongestCycle(edges);

        Assert.Equal(3, longest);
    }

    [Fact]
    public void LongestCycle_ChainEndsAtADeadEnd_ReturnsNegativeOne()
    {
        // 0 -> 2 -> 3 -> 1 -> (no outgoing edge). No cycle anywhere.
        int[] edges = [2, -1, 3, 1];

        var longest = LongestCycle(edges);

        Assert.Equal(-1, longest);
    }

    [Fact]
    public void LongestCycle_TwoDisjointCyclesOfDifferentLengths_ReturnsTheLongerOne()
    {
        // 0 -> 1 -> 2 -> 0 (length 3), 3 -> 4 -> 3 (length 2).
        int[] edges = [1, 2, 0, 4, 3];

        var longest = LongestCycle(edges);

        Assert.Equal(3, longest);
    }

    private static int LongestCycle(int[] edges)
    {
        var nodes = BuildFunctionalGraph(edges);
        var components = StronglyConnectedComponents.Tarjan<
            FunctionalGraphNode, FunctionalGraphTopology, ListChildren<FunctionalGraphNode>,
            NaturalChildOrder<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>, ListChildren<FunctionalGraphNode>>(nodes);

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

    private static FunctionalGraphNode[] BuildFunctionalGraph(int[] edges)
    {
        var nodes = new FunctionalGraphNode[edges.Length];

        for (var i = 0; i < edges.Length; i++)
        {
            nodes[i] = new FunctionalGraphNode(i);
        }

        for (var i = 0; i < edges.Length; i++)
        {
            if (edges[i] != -1)
            {
                nodes[i].Successors.Add(nodes[edges[i]]);
            }
        }

        return nodes;
    }
}
