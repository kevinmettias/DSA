using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

public sealed class ListChildrenTests
{
    [Fact]
    public void Count_ReportsTheBackingListsLength() => Assert.Equal(2, new ListChildren<TestNode>([new("A"), new("B")]).Count);

    [Fact]
    public void Count_EmptyList_IsZero() => Assert.Equal(0, new ListChildren<TestNode>([]).Count);

    [Fact]
    public void Get_ReturnsItemsInTheBackingListsOrder()
    {
        var children = new ListChildren<TestNode>([new("A"), new("B"), new("C")]);

        Assert.Equal(["A", "B", "C"], Enumerable.Range(0, children.Count).Select(i => children.Get(i).Name));
    }

    [Fact]
    public void Count_ViewsTheLiveListRatherThanACopy()
    {
        // The struct wraps the caller's List by reference; a later Add is visible.
        var items = new List<TestNode> { new("A") };
        var children = new ListChildren<TestNode>(items);

        items.Add(new TestNode("B"));

        Assert.Equal(2, children.Count);
    }

    [Fact]
    public void Get_IndexPastTheEnd_Throws()
    {
        var children = new ListChildren<TestNode>([new("A")]);

        Assert.Throws<ArgumentOutOfRangeException>(() => children.Get(1));
    }
}
