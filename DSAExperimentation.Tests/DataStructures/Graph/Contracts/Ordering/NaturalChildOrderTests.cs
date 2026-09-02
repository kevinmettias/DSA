using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

public sealed class NaturalChildOrderTests
{
    [Fact]
    public void Apply_LeavesTheChildrenInTheirDeclaredOrder()
    {
        var children = new ListChildren<TestNode>([new("A"), new("B"), new("C")]);

        var ordered = NaturalChildOrder<TestNode, ListChildren<TestNode>>.Apply(children);

        Assert.Equal(["A", "B", "C"], Enumerable.Range(0, ordered.Count).Select(i => ordered.Get(i).Name));
    }

    [Fact]
    public void Apply_PreservesTheCount()
    {
        var children = new ListChildren<TestNode>([new("A"), new("B")]);

        Assert.Equal(children.Count, NaturalChildOrder<TestNode, ListChildren<TestNode>>.Apply(children).Count);
    }

    [Fact]
    public void Apply_EmptyChildren_StaysEmpty()
    {
        var children = new ListChildren<TestNode>([]);

        Assert.Equal(0, NaturalChildOrder<TestNode, ListChildren<TestNode>>.Apply(children).Count);
    }
}
