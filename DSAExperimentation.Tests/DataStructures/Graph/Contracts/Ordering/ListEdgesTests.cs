using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

public sealed class ListEdgesTests
{
    private static ListEdges<TestNode, int> Edges() =>
        new([(3, new TestNode("A")), (7, new TestNode("B"))]);

    [Fact]
    public void Count_ReportsTheBackingListsLength() => Assert.Equal(2, Edges().Count);

    [Fact]
    public void Count_EmptyList_IsZero() => Assert.Equal(0, new ListEdges<TestNode, int>([]).Count);

    [Fact]
    public void Get_ReturnsEachEdgesDataAndTargetTogether()
    {
        var edges = Edges();

        Assert.Equal(3, edges.Get(0).Data);
        Assert.Equal("A", edges.Get(0).Target.Name);
        Assert.Equal(7, edges.Get(1).Data);
        Assert.Equal("B", edges.Get(1).Target.Name);
    }

    [Fact]
    public void Get_PreservesTheBackingListsOrder()
    {
        var edges = Edges();

        Assert.Equal([3, 7], Enumerable.Range(0, edges.Count).Select(i => edges.Get(i).Data));
    }
}
