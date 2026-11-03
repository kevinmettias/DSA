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

    [Fact]
    public void To_ShortcutEdge_ReportsTheFewestEdges()
    {
        var nodes = SampleGraph();

        Assert.Equal(2, To(nodes[0], nodes[3]));
    }

    [Fact]
    public void To_RootItself_IsZero()
    {
        var nodes = SampleGraph();

        Assert.Equal(0, To(nodes[0], nodes[0]));
    }

    [Fact]
    public void To_UnreachableTarget_IsNull()
    {
        var nodes = SampleGraph();

        Assert.Null(To(nodes[0], nodes[4]));
    }

    // Roots 0 and 4 of an undirected path 0 - 1 - 2 - 3 - 4: each node's distance is to the nearer end.
    [Fact]
    public void FromAll_TwoRoots_MeasuresEachNodeFromTheNearest()
    {
        var path = UndirectedPath(5);

        var distances = BreadthFirstDistances.FromAll<AdjacencyNode, AdjacencyTopology, ListChildren<AdjacencyNode>>([path[0], path[4]]);

        Assert.Equal([0, 1, 2, 1, 0], path.Select(node => distances[node]));
    }

    [Fact]
    public void FromAll_RepeatedRoot_StartsOneSearch()
    {
        var path = UndirectedPath(3);

        var distances = BreadthFirstDistances.FromAll<AdjacencyNode, AdjacencyTopology, ListChildren<AdjacencyNode>>([path[0], path[0]]);

        Assert.Equal([0, 1, 2], path.Select(node => distances[node]));
    }

    [Fact]
    public void FromAll_NoRoots_IsEmpty()
        => Assert.Empty(BreadthFirstDistances.FromAll<AdjacencyNode, AdjacencyTopology, ListChildren<AdjacencyNode>>([]));

    private static int? To(AdjacencyNode root, AdjacencyNode target) =>
        BreadthFirstDistances.To<AdjacencyNode, AdjacencyTopology, ListChildren<AdjacencyNode>>(root, target);

    private static AdjacencyNode[] UndirectedPath(int length)
    {
        var nodes = Enumerable.Range(0, length).Select(id => new AdjacencyNode(id)).ToArray();

        for (var i = 1; i < length; i++)
        {
            nodes[i - 1].Neighbors.Add(nodes[i]);
            nodes[i].Neighbors.Add(nodes[i - 1]);
        }

        return nodes;
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
