using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

public sealed class EdgeTargetsTests
{
    private static EdgeTargets<TestNode, int, ListEdges<TestNode, int>> Targets() =>
        new(new ListEdges<TestNode, int>([(3, new TestNode("A")), (7, new TestNode("B"))]));

    [Fact]
    public void Count_MatchesTheWrappedEdgeCount()
    {
        Assert.Equal(2, Targets().Count);
    }

    [Fact]
    public void Get_DropsTheEdgeDataAndKeepsTheTarget()
    {
        var targets = Targets();

        Assert.Equal(["A", "B"], Enumerable.Range(0, targets.Count).Select(i => targets.Get(i).Name));
    }

    [Fact]
    public void Count_NoEdges_IsZero()
    {
        var targets = new EdgeTargets<TestNode, int, ListEdges<TestNode, int>>(new ListEdges<TestNode, int>([]));

        Assert.Equal(0, targets.Count);
    }
}
