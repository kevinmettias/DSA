using DSAExperimentation.Algorithms.MinimumSpanningTrees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.MinimumSpanningTrees;

public sealed partial class MinimumSpanningTreeTests
{
    [Fact]
    public void Kruskal_ConnectedGraphWithCycle_ReturnsMinimumWeightSpanningTree()
    {
        var graph = BuildConnectedGraphWithCycle();

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [graph.A, graph.B, graph.C, graph.D]);

        AssertIsTheMinimumWeightSpanningTreeOfTheCycle(mst, graph);
    }

    private static void AssertIsTheMinimumWeightSpanningTreeOfTheCycle(
        List<(WeightedNode A, WeightedNode B, int Weight)> mst,
        (WeightedNode A, WeightedNode B, WeightedNode C, WeightedNode D) graph)
    {
        Assert.Equal(Fixtures.CycleEdgeCount, mst.Count);
        Assert.Equal(Fixtures.CycleTotalWeight, mst.Sum(edge => edge.Weight));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, graph.A, graph.C));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, graph.B, graph.D));
    }

    private static (WeightedNode A, WeightedNode B, WeightedNode C, WeightedNode D) BuildConnectedGraphWithCycle()
    {
        var a = new WeightedNode(Fixtures.NodeA);
        var b = new WeightedNode(Fixtures.NodeB);
        var c = new WeightedNode(Fixtures.NodeC);
        var d = new WeightedNode(Fixtures.NodeD);
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(b, c, Fixtures.CycleWeightBToC);
        AddUndirectedEdge(a, c, Fixtures.CycleWeightAToC);
        AddUndirectedEdge(c, d, 1);
        AddUndirectedEdge(b, d, Fixtures.CycleWeightBToD);

        return (a, b, c, d);
    }

    [Fact]
    public void Kruskal_Triangle_ExcludesHeaviestEdge()
    {
        var a = new WeightedNode(Fixtures.NodeA);
        var b = new WeightedNode(Fixtures.NodeB);
        var c = new WeightedNode(Fixtures.NodeC);
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(b, c, 1);
        AddUndirectedEdge(a, c, Fixtures.TriangleWeightAToC);

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c]);

        Assert.Equal(Fixtures.TriangleEdgeCount, mst.Count);
        Assert.Equal(Fixtures.TriangleTotalWeight, mst.Sum(edge => edge.Weight));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, c));
    }

    [Fact]
    public void Kruskal_DisconnectedGraph_ReturnsSpanningForest()
    {
        var a = new WeightedNode(Fixtures.NodeA);
        var b = new WeightedNode(Fixtures.NodeB);
        var c = new WeightedNode(Fixtures.NodeC);
        var d = new WeightedNode(Fixtures.NodeD);
        AddUndirectedEdge(a, b, 1);
        AddUndirectedEdge(c, d, 1);

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c, d]);

        Assert.Equal(Fixtures.ForestEdgeCount, mst.Count);
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, c));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, a, d));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, b, c));
        Assert.DoesNotContain(mst, edge => IsEdgeBetween(edge, b, d));
    }

    [Fact]
    public void Kruskal_SingleVertexWithNoEdges_ReturnsEmptyResult()
    {
        var isolated = new WeightedNode(Fixtures.NodeZ);

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

    /// <summary>
    /// The node labels, edge weights and expected result sizes these tests use, named
    /// once so a second test does not have to reach into a neighbour's body for them.
    /// </summary>
    private static class Fixtures
    {
        public const string NodeA = "A";
        public const string NodeB = "B";
        public const string NodeC = "C";
        public const string NodeD = "D";
        public const string NodeZ = "Z";

        public const int CycleWeightBToC = 2;
        public const int CycleWeightAToC = 4;
        public const int CycleWeightBToD = 5;
        public const int CycleEdgeCount = 3;
        public const int CycleTotalWeight = 4;

        public const int TriangleWeightAToC = 5;
        public const int TriangleEdgeCount = 2;
        public const int TriangleTotalWeight = 2;

        public const int ForestEdgeCount = 2;
    }
}
