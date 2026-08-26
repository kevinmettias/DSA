using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;
using BridgesAndArticulationPointsOperations = DSAExperimentation.Algorithms.Connectivity.BridgesAndArticulationPoints;

namespace DSAExperimentation.Tests.Algorithms.Connectivity;

public sealed partial class BridgesAndArticulationPointsTests
{
    private static void Connect(TestNode first, TestNode second)
    {
        first.Children.Add(second);
        second.Children.Add(first);
    }

    // Bridge/articulation-point membership, not discovery order, is the contract this
    // algorithm promises - both normalizers sort to a stable order rather than asserting the
    // algorithm's internal DFS order.
    private static List<List<string>> NormalizeBridges(List<(TestNode A, TestNode B)> bridges)
    {
        var normalized = bridges
            .Select(bridge => new List<string> { bridge.A.Name, bridge.B.Name }.OrderBy(name => name).ToList())
            .OrderBy(pair => pair[0])
            .ThenBy(pair => pair[1])
            .ToList();

        return normalized;
    }

    private static List<string> NormalizeArticulationPoints(List<TestNode> points)
    {
        var normalized = points.Select(point => point.Name).OrderBy(name => name).ToList();
        return normalized;
    }

    [Fact]
    public void Find_Path_ReturnsBothEdgesAsBridges()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        Connect(a, b);
        Connect(b, c);

        var (bridges, _) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c]);

        Assert.Equal([new List<string> { "A", "B" }, new List<string> { "B", "C" }], NormalizeBridges(bridges));
    }

    [Fact]
    public void Find_Path_ReturnsMiddleNodeAsOnlyArticulationPoint()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        Connect(a, b);
        Connect(b, c);

        var (_, articulationPoints) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c]);

        Assert.Equal(["B"], NormalizeArticulationPoints(articulationPoints));
    }

    [Fact]
    public void Find_Triangle_ReturnsNoBridges()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        Connect(a, b);
        Connect(b, c);
        Connect(c, a);

        var (bridges, _) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c]);

        Assert.Empty(bridges);
    }

    [Fact]
    public void Find_Triangle_ReturnsNoArticulationPoints()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        Connect(a, b);
        Connect(b, c);
        Connect(c, a);

        var (_, articulationPoints) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c]);

        Assert.Empty(articulationPoints);
    }

    private static (TestNode A, TestNode B, TestNode C, TestNode D, TestNode E, TestNode F) BuildTwoTrianglesJoinedByOneEdge()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");
        var e = new TestNode("E");
        var f = new TestNode("F");
        Connect(a, b);
        Connect(b, c);
        Connect(c, a);
        Connect(d, e);
        Connect(e, f);
        Connect(f, d);
        Connect(c, d);

        return (a, b, c, d, e, f);
    }

    [Fact]
    public void Find_TwoTrianglesJoinedByOneEdge_ReturnsThatEdgeAsBridge()
    {
        var (a, b, c, d, e, f) = BuildTwoTrianglesJoinedByOneEdge();

        var (bridges, _) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c, d, e, f]);

        Assert.Equal([new List<string> { "C", "D" }], NormalizeBridges(bridges));
    }

    [Fact]
    public void Find_TwoTrianglesJoinedByOneEdge_ReturnsBothEndpointsAsArticulationPoints()
    {
        var (a, b, c, d, e, f) = BuildTwoTrianglesJoinedByOneEdge();

        var (_, articulationPoints) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c, d, e, f]);

        Assert.Equal(["C", "D"], NormalizeArticulationPoints(articulationPoints));
    }

    // Two parallel edges between the same pair - the parent-skip-once handling must treat
    // the second occurrence as a genuine back edge rather than silently ignoring it as
    // "the edge just arrived on," or this would be misreported as a bridge.
    [Fact]
    public void Find_ParallelEdgeBetweenSamePair_IsNotReportedAsABridge()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        a.Children.Add(b);
        a.Children.Add(b);
        b.Children.Add(a);
        b.Children.Add(a);

        var (bridges, _) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b]);

        Assert.Empty(bridges);
    }

    [Fact]
    public void Find_ParallelEdgeBetweenSamePair_ProducesNoArticulationPoints()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        a.Children.Add(b);
        a.Children.Add(b);
        b.Children.Add(a);
        b.Children.Add(a);

        var (_, articulationPoints) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b]);

        Assert.Empty(articulationPoints);
    }

    private static (TestNode Center, TestNode LeafOne, TestNode LeafTwo, TestNode LeafThree) BuildStarGraph()
    {
        var center = new TestNode("Center");
        var leafOne = new TestNode("L1");
        var leafTwo = new TestNode("L2");
        var leafThree = new TestNode("L3");
        Connect(center, leafOne);
        Connect(center, leafTwo);
        Connect(center, leafThree);

        return (center, leafOne, leafTwo, leafThree);
    }

    [Fact]
    public void Find_StarGraph_ReturnsEveryEdgeAsBridge()
    {
        var (center, leafOne, leafTwo, leafThree) = BuildStarGraph();

        var (bridges, _) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [center, leafOne, leafTwo, leafThree]);

        Assert.Equal(
            [
                new List<string> { "Center", "L1" },
                new List<string> { "Center", "L2" },
                new List<string> { "Center", "L3" },
            ],
            NormalizeBridges(bridges));
    }

    [Fact]
    public void Find_StarGraph_ReturnsCenterAsOnlyArticulationPoint()
    {
        var (center, leafOne, leafTwo, leafThree) = BuildStarGraph();

        var (_, articulationPoints) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [center, leafOne, leafTwo, leafThree]);

        Assert.Equal(["Center"], NormalizeArticulationPoints(articulationPoints));
    }

    [Fact]
    public void Find_SingleNode_ReturnsNoBridges()
    {
        var a = new TestNode("A");

        var (bridges, _) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a]);

        Assert.Empty(bridges);
    }

    [Fact]
    public void Find_SingleNode_ReturnsNoArticulationPoints()
    {
        var a = new TestNode("A");

        var (_, articulationPoints) = BridgesAndArticulationPointsOperations.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a]);

        Assert.Empty(articulationPoints);
    }
}
