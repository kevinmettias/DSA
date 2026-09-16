using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed partial class AllPairsShortestPathsTests
{
    [Fact]
    public void TryComputeDistances_SampleGraph_MatchesDijkstraFromEachSource()
    {
        var (a, b, c, d) = WeightedGraphs.SampleGraph();

        var succeeded = TryAllPairs([a, b, c, d], out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[(a, a)]);
        Assert.Equal(1, distances[(a, b)]);
        Assert.Equal(Fixtures.SampleExpectedDistanceAToC, distances[(a, c)]); // A-B-C = 1+2, cheaper than direct A-C = 4
        Assert.Equal(Fixtures.SampleExpectedDistanceAToD, distances[(a, d)]); // A-B-C-D = 1+2+1
        Assert.Equal(Fixtures.SampleExpectedDistanceBToC, distances[(b, c)]);
        Assert.Equal(Fixtures.SampleExpectedDistanceBToD, distances[(b, d)]); // B-C-D = 2+1, cheaper than direct B-D = 5
        Assert.Equal(1, distances[(c, d)]);
    }

    [Fact]
    public void TryComputeDistances_UnreachablePair_IsAbsentFromResult()
    {
        var (a, b, _, _) = WeightedGraphs.SampleGraph();

        var succeeded = TryAllPairs([a, b], out var distances);

        Assert.False(distances.ContainsKey((b, a)));
    }

    // Same negative-but-acyclic shape BellmanFordTests uses: the cheapest A->C only shows
    // up if the triple loop actually relaxes across a negative-weight edge.
    [Fact]
    public void TryComputeDistances_NegativeEdgeWithoutCycle_ComputesCorrectDistances()
    {
        var a = new WeightedNode(Fixtures.NodeA);
        var b = new WeightedNode(Fixtures.NodeB);
        var c = new WeightedNode(Fixtures.NodeC);
        a.Edges.Add((Fixtures.NegativeEdgeWeightAToB, b));
        b.Edges.Add((Fixtures.NegativeEdgeWeightBToC, c));

        var succeeded = TryAllPairs([a, b, c], out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[(a, a)]);
        Assert.Equal(Fixtures.NegativeEdgeWeightAToB, distances[(a, b)]);
        Assert.Equal(Fixtures.NegativeEdgeExpectedDistanceAToC, distances[(a, c)]);
    }

    // Same cycle BellmanFordTests uses (B -> C -> B totalling -2), reachable from A. The
    // diagonal dist[B,B]/dist[C,C] is what goes negative - the failure signal itself, which
    // is exactly why a self-pair can't be hardcoded to always report TWeight.Zero.
    [Fact]
    public void TryComputeDistances_NegativeCycle_ReturnsFalse()
    {
        var a = new WeightedNode(Fixtures.NodeA);
        var b = new WeightedNode(Fixtures.NodeB);
        var c = new WeightedNode(Fixtures.NodeC);
        a.Edges.Add((1, b));
        b.Edges.Add((1, c));
        c.Edges.Add((Fixtures.NegativeCycleWeightCToB, b));

        var succeeded = TryAllPairs([a, b, c], out var distances);

        Assert.False(succeeded);
        Assert.True(distances[(b, b)] < 0);
    }

    // A's edge to C exists on the node itself, but C is omitted from the vertices given to
    // this call - BuildInitialMatrix must skip it via index.TryGetValue rather than throw
    // KeyNotFoundException indexing straight into `index`.
    [Fact]
    public void TryComputeDistances_EdgeOutsideGivenVertices_IsSkippedNotThrown()
    {
        var a = new WeightedNode(Fixtures.NodeA);
        var b = new WeightedNode(Fixtures.NodeB);
        var c = new WeightedNode(Fixtures.NodeC);
        a.Edges.Add((1, b));
        a.Edges.Add((1, c));

        var succeeded = TryAllPairs([a, b], out var distances);

        Assert.True(succeeded);
        Assert.Equal(1, distances[(a, b)]);
        Assert.DoesNotContain(distances.Keys, pair => pair.From == c || pair.To == c);
    }

    // int.MaxValue is BuildInitialMatrix's own "still unreached" sentinel; an edge weighted
    // exactly that would be indistinguishable from no edge in every later read of that cell,
    // so it's validated rather than silently accepted.
    [Fact]
    public void TryComputeDistances_EdgeWeightEqualsReservedSentinel_Throws()
    {
        var a = new WeightedNode(Fixtures.NodeA);
        var b = new WeightedNode(Fixtures.NodeB);
        a.Edges.Add((int.MaxValue, b));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TryAllPairs([a, b], out _));
    }

    // A-B and B-C are each representable, but their sum during Refine's relaxation
    // underflows int - without a checked-arithmetic guard, that would silently wrap to a
    // large positive number that reads as a (very wrong) real distance. The correct
    // behavior is to treat the overflowed candidate as unusable, leaving (A,C) absent
    // rather than reporting a corrupted value.
    [Fact]
    public void TryComputeDistances_RelaxationOverflows_PairIsAbsentNotCorrupted()
    {
        var a = new WeightedNode(Fixtures.NodeA);
        var b = new WeightedNode(Fixtures.NodeB);
        var c = new WeightedNode(Fixtures.NodeC);
        a.Edges.Add((Fixtures.RelaxationOverflowEdgeWeight, b));
        b.Edges.Add((Fixtures.RelaxationOverflowEdgeWeight, c));

        var succeeded = TryAllPairs([a, b, c], out var distances);

        Assert.True(succeeded);
        Assert.Equal(Fixtures.RelaxationOverflowEdgeWeight, distances[(a, b)]);
        Assert.Equal(Fixtures.RelaxationOverflowEdgeWeight, distances[(b, c)]);
        Assert.False(distances.ContainsKey((a, c)));
    }

    // The eight-type argument list is identical at every call site, so the entry point is
    // named once here rather than spelled out in each test.
    private static bool TryAllPairs(
        WeightedNode[] vertices,
        out Dictionary<(WeightedNode From, WeightedNode To), int> distances) =>
        AllPairsShortestPaths.TryComputeDistances<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(vertices, out distances);

    /// <summary>
    /// The node labels, edge weights and expected distances these tests use, named once
    /// so a second test does not have to reach into a neighbour's body for them.
    /// </summary>
    private static class Fixtures
    {
        public const string NodeA = "A";
        public const string NodeB = "B";
        public const string NodeC = "C";

        public const int SampleExpectedDistanceAToC = 3;
        public const int SampleExpectedDistanceAToD = 4;
        public const int SampleExpectedDistanceBToC = 2;
        public const int SampleExpectedDistanceBToD = 3;

        public const int NegativeEdgeWeightAToB = 4;
        public const int NegativeEdgeWeightBToC = -2;
        public const int NegativeEdgeExpectedDistanceAToC = 2;
        public const int NegativeCycleWeightCToB = -3;

        public const int RelaxationOverflowEdgeWeight = -1_500_000_000;
    }
}
