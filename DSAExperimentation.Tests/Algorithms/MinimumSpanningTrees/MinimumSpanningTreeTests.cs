using DSAExperimentation.Algorithms.MinimumSpanningTrees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.MinimumSpanningTrees;

public sealed partial class MinimumSpanningTreeTests
{
    private const string NodeA = "A";
    private const string NodeB = "B";
    private const string NodeC = "C";
    private const string NodeD = "D";
    private const string NodeZ = "Z";

    [Fact]
    public void Kruskal_ConnectedGraphWithCycle_ReturnsMinimumWeightSpanningTree()
    {
        const int weightBToC = 2;
        const int weightAToC = 4;
        const int weightBToD = 5;
        const int expectedEdgeCount = 3;
        const int expectedTotalWeight = 4;

        var (a, b, c, d) = BuildConnectedGraphWithCycle(weightBToC, weightAToC, weightBToD);

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c, d]);

        Assert.Equal(expectedEdgeCount, mst.Count);
        Assert.Equal(expectedTotalWeight, mst.Sum(edge => edge.Weight));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, c));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, b, d));
    }

    private static (WeightedNode A, WeightedNode B, WeightedNode C, WeightedNode D) BuildConnectedGraphWithCycle(
        int weightBToC, int weightAToC, int weightBToD)
    {
        var a = new WeightedNode(NodeA);
        var b = new WeightedNode(NodeB);
        var c = new WeightedNode(NodeC);
        var d = new WeightedNode(NodeD);
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(b, c, weightBToC);
        AddUndirectedEdge(a, c, weightAToC);
        AddUndirectedEdge(c, d, 1);
        AddUndirectedEdge(b, d, weightBToD);

        return (a, b, c, d);
    }

    [Fact]
    public void Kruskal_Triangle_ExcludesHeaviestEdge()
    {
        const int weightAToC = 5;
        const int expectedEdgeCount = 2;
        const int expectedTotalWeight = 2;

        var a = new WeightedNode(NodeA);
        var b = new WeightedNode(NodeB);
        var c = new WeightedNode(NodeC);
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(b, c, 1);
        AddUndirectedEdge(a, c, weightAToC);

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c]);

        Assert.Equal(expectedEdgeCount, mst.Count);
        Assert.Equal(expectedTotalWeight, mst.Sum(edge => edge.Weight));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, c));
    }

    [Fact]
    public void Kruskal_DisconnectedGraph_ReturnsSpanningForest()
    {
        const int expectedEdgeCount = 2;

        var a = new WeightedNode(NodeA);
        var b = new WeightedNode(NodeB);
        var c = new WeightedNode(NodeC);
        var d = new WeightedNode(NodeD);
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(c, d, 1);

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c, d]);

        Assert.Equal(expectedEdgeCount, mst.Count);
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, c));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, d));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, b, c));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, b, d));
    }

    [Fact]
    public void Kruskal_SingleVertexWithNoEdges_ReturnsEmptyResult()
    {
        var isolated = new WeightedNode(NodeZ);

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
