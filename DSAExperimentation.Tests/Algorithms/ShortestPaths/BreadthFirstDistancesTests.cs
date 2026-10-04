using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Adjacency;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

// 0 -> 1 -> 2 -> 3, plus a shortcut 0 -> 2 and a back edge 3 -> 0, and a node 4 nothing reaches. The
// shortcut is what separates breadth-first depth from the first path a depth-first walk would find
// (0, 1, 2 reaches 2 at depth 2; the shortcut reaches it at 1), and the back edge is a cycle the walk
// must not loop on.
public sealed partial class BreadthFirstDistancesTests
{
    [Fact]
    public void From_Root_IsAtDistanceZero()
    {
        var nodes = SampleGraph();

        var distances = From(nodes[0]);

        Assert.Equal(0, distances[nodes[0]]);
    }

    [Fact]
    public void From_ShortcutEdge_ReportsTheFewestEdges()
    {
        var nodes = SampleGraph();

        var distances = From(nodes[0]);

        Assert.Equal(1, distances[nodes[2]]);
        Assert.Equal(2, distances[nodes[3]]);
    }

    [Fact]
    public void From_Cycle_VisitsEachNodeOnce()
    {
        var nodes = SampleGraph();

        var distances = From(nodes[0]);

        Assert.Equal(4, distances.Count);
    }

    [Fact]
    public void From_UnreachableNode_IsAbsentFromTheMap()
    {
        var nodes = SampleGraph();

        var distances = From(nodes[0]);

        Assert.False(distances.ContainsKey(nodes[4]));
    }

    private static Dictionary<AdjacencyNode, int> From(AdjacencyNode root) =>
        BreadthFirstDistances.From<AdjacencyNode, AdjacencyTopology, ListChildren<AdjacencyNode>>(root);

    private static AdjacencyNode[] SampleGraph()
    {
        var nodes = Enumerable.Range(0, 5).Select(id => new AdjacencyNode(id)).ToArray();
        nodes[0].Neighbors.AddRange([nodes[1], nodes[2]]);
        nodes[1].Neighbors.Add(nodes[2]);
        nodes[2].Neighbors.Add(nodes[3]);
        nodes[3].Neighbors.Add(nodes[0]);
        return nodes;
    }
}
