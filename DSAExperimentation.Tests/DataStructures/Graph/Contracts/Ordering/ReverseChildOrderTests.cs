using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

public sealed partial class ReverseChildOrderTests
{
    [Fact]
    public void Apply_PresentsTheChildrenBackToFront()
    {
        var children = new ListChildren<TestNode>([new("A"), new("B"), new("C")]);

        var ordered = ReverseChildOrder<TestNode, ListChildren<TestNode>>.Apply(children);

        Assert.Equal(["C", "B", "A"], Enumerable.Range(0, ordered.Count).Select(i => ordered.Get(i).Name));
    }

    [Fact]
    public void Apply_PreservesTheCount()
    {
        var children = new ListChildren<TestNode>([new("A"), new("B"), new("C")]);

        Assert.Equal(children.Count, ReverseChildOrder<TestNode, ListChildren<TestNode>>.Apply(children).Count);
    }

    [Fact]
    public void Apply_EmptyChildren_StaysEmpty()
    {
        var children = new ListChildren<TestNode>([]);

        Assert.Equal(0, ReverseChildOrder<TestNode, ListChildren<TestNode>>.Apply(children).Count);
    }
}
