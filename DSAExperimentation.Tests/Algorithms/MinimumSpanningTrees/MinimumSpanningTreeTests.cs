using DSAExperimentation.Algorithms.MinimumSpanningTrees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.MinimumSpanningTrees;

public sealed partial class MinimumSpanningTreeTests
{
    [Fact]
    public void Kruskal_ConnectedGraphWithCycle_ReturnsMinimumWeightSpanningTree()
    {
        var a = new WeightedNode("A");
        var b = new WeightedNode("B");
        var c = new WeightedNode("C");
        var d = new WeightedNode("D");
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(b, c, 2);
        AddUndirectedEdge(a, c, 4);
        AddUndirectedEdge(c, d, 1);
        AddUndirectedEdge(b, d, 5);

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c, d]);

        Assert.Equal(3, mst.Count);
        Assert.Equal(4, mst.Sum(edge => edge.Weight));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, c));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, b, d));
    }

    [Fact]
    public void Kruskal_Triangle_ExcludesHeaviestEdge()
    {
        var a = new WeightedNode("A");
        var b = new WeightedNode("B");
        var c = new WeightedNode("C");
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(b, c, 1);
        AddUndirectedEdge(a, c, 5);

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c]);

        Assert.Equal(2, mst.Count);
        Assert.Equal(2, mst.Sum(edge => edge.Weight));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, c));
    }

    [Fact]
    public void Kruskal_DisconnectedGraph_ReturnsSpanningForest()
    {
        var a = new WeightedNode("A");
        var b = new WeightedNode("B");
        var c = new WeightedNode("C");
        var d = new WeightedNode("D");
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(c, d, 1);

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c, d]);

        Assert.Equal(2, mst.Count);
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, c));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, d));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, b, c));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, b, d));
    }

    [Fact]
    public void Kruskal_SingleVertexWithNoEdges_ReturnsEmptyResult()
    {
        var isolated = new WeightedNode("Z");

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [isolated]);

        Assert.Empty(mst);
    }

    private static void AddUndirectedEdge(WeightedNode first, WeightedNode second, int weight)
    {
        first.Edges.Add((weight, second));
        second.Edges.Add((weight, first));
    }

    private static bool IsEdgeBetween(
        (WeightedNode A, WeightedNode B, int Weight) edge, WeightedNode first, WeightedNode second)
        => (edge.A == first && edge.B == second) || (edge.A == second && edge.B == first);
}
