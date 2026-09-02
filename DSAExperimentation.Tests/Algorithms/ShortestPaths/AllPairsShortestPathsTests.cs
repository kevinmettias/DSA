using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed class AllPairsShortestPathsTests
{
    private const string NodeLabelA = "A";
    private const string NodeLabelB = "B";
    private const string NodeLabelC = "C";

    [Fact]
    public void TryComputeDistances_SampleGraph_MatchesDijkstraFromEachSource()
    {
        const int ExpectedDistanceAtoC = 3;
        const int ExpectedDistanceAtoD = 4;
        const int ExpectedDistanceBtoC = 2;
        const int ExpectedDistanceBtoD = 3;

        var (a, b, c, d) = WeightedGraphs.SampleGraph();

        var succeeded = AllPairsShortestPaths.TryComputeDistances<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c, d], out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[(a, a)]);
        Assert.Equal(1, distances[(a, b)]);
        Assert.Equal(ExpectedDistanceAtoC, distances[(a, c)]); // A-B-C = 1+2, cheaper than direct A-C = 4
        Assert.Equal(ExpectedDistanceAtoD, distances[(a, d)]); // A-B-C-D = 1+2+1
        Assert.Equal(ExpectedDistanceBtoC, distances[(b, c)]);
        Assert.Equal(ExpectedDistanceBtoD, distances[(b, d)]); // B-C-D = 2+1, cheaper than direct B-D = 5
        Assert.Equal(1, distances[(c, d)]);
    }

    [Fact]
    public void TryComputeDistances_UnreachablePair_IsAbsentFromResult()
    {
        var (a, b, _, _) = WeightedGraphs.SampleGraph();

        var succeeded = AllPairsShortestPaths.TryComputeDistances<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b], out var distances);

        Assert.False(distances.ContainsKey((b, a)));
    }

    // Same negative-but-acyclic shape BellmanFordTests uses: the cheapest A->C only shows
    // up if the triple loop actually relaxes across a negative-weight edge.
    [Fact]
    public void TryComputeDistances_NegativeEdgeWithoutCycle_ComputesCorrectDistances()
    {
        const int EdgeWeightAtoB = 4;
        const int EdgeWeightBtoC = -2;
        const int ExpectedDistanceAtoC = 2;

        var a = new WeightedNode(NodeLabelA);
        var b = new WeightedNode(NodeLabelB);
        var c = new WeightedNode(NodeLabelC);
        a.Edges.Add((EdgeWeightAtoB, b));
        b.Edges.Add((EdgeWeightBtoC, c));

        var succeeded = AllPairsShortestPaths.TryComputeDistances<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c], out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[(a, a)]);
        Assert.Equal(EdgeWeightAtoB, distances[(a, b)]);
        Assert.Equal(ExpectedDistanceAtoC, distances[(a, c)]);
    }

    // Same cycle BellmanFordTests uses (B -> C -> B totalling -2), reachable from A. The
    // diagonal dist[B,B]/dist[C,C] is what goes negative - the failure signal itself, which
    // is exactly why a self-pair can't be hardcoded to always report TWeight.Zero.
    [Fact]
    public void TryComputeDistances_NegativeCycle_ReturnsFalse()
    {
        const int EdgeWeightCtoB = -3;

        var a = new WeightedNode(NodeLabelA);
        var b = new WeightedNode(NodeLabelB);
        var c = new WeightedNode(NodeLabelC);
        a.Edges.Add((1, b));
        b.Edges.Add((1, c));
        c.Edges.Add((EdgeWeightCtoB, b));

        var succeeded = AllPairsShortestPaths.TryComputeDistances<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c], out var distances);

        Assert.False(succeeded);
        Assert.True(distances[(b, b)] < 0);
    }

    // A's edge to C exists on the node itself, but C is omitted from the vertices given to
    // this call - BuildInitialMatrix must skip it via index.TryGetValue rather than throw
    // KeyNotFoundException indexing straight into `index`.
    [Fact]
    public void TryComputeDistances_EdgeOutsideGivenVertices_IsSkippedNotThrown()
    {
        var a = new WeightedNode(NodeLabelA);
        var b = new WeightedNode(NodeLabelB);
        var c = new WeightedNode(NodeLabelC);
        a.Edges.Add((1, b));
        a.Edges.Add((1, c));

        var succeeded = AllPairsShortestPaths.TryComputeDistances<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b], out var distances);

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
        var a = new WeightedNode(NodeLabelA);
        var b = new WeightedNode(NodeLabelB);
        a.Edges.Add((int.MaxValue, b));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AllPairsShortestPaths.TryComputeDistances<
                WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
                [a, b], out _));
    }

    // A-B and B-C are each representable, but their sum during Refine's relaxation
    // underflows int - without a checked-arithmetic guard, that would silently wrap to a
    // large positive number that reads as a (very wrong) real distance. The correct
    // behavior is to treat the overflowed candidate as unusable, leaving (A,C) absent
    // rather than reporting a corrupted value.
    [Fact]
    public void TryComputeDistances_RelaxationOverflows_PairIsAbsentNotCorrupted()
    {
        const int LargeNegativeEdgeWeight = -1_500_000_000;

        var a = new WeightedNode(NodeLabelA);
        var b = new WeightedNode(NodeLabelB);
        var c = new WeightedNode(NodeLabelC);
        a.Edges.Add((LargeNegativeEdgeWeight, b));
        b.Edges.Add((LargeNegativeEdgeWeight, c));

        var succeeded = AllPairsShortestPaths.TryComputeDistances<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c], out var distances);

        Assert.True(succeeded);
        Assert.Equal(LargeNegativeEdgeWeight, distances[(a, b)]);
        Assert.Equal(LargeNegativeEdgeWeight, distances[(b, c)]);
        Assert.False(distances.ContainsKey((a, c)));
    }
}
