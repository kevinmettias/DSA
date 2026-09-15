using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

public sealed class ReversedChildrenTests
{
    private static ReversedChildren<TestNode, ListChildren<TestNode>> Wrap(params string[] names) =>
        new(new ListChildren<TestNode>([.. names.Select(n => new TestNode(n))]));

    [Fact]
    public void Count_MatchesTheWrappedChildren() => Assert.Equal(3, Wrap("A", "B", "C").Count);

    [Fact]
    public void Get_ReturnsTheWrappedChildrenBackToFront()
    {
        var children = Wrap("A", "B", "C");

        Assert.Equal(["C", "B", "A"], Enumerable.Range(0, children.Count).Select(i => children.Get(i).Name));
    }

    [Fact]
    public void Get_SingleChild_IsUnchanged() => Assert.Equal("A", Wrap("A").Get(0).Name);

    [Fact]
    public void Get_AppliedTwice_RestoresTheOriginalOrder()
    {
        var once = Wrap("A", "B", "C");
        var twice = new ReversedChildren<TestNode, ReversedChildren<TestNode, ListChildren<TestNode>>>(once);

        Assert.Equal(["A", "B", "C"], Enumerable.Range(0, twice.Count).Select(i => twice.Get(i).Name));
    }

    [Fact]
    public void Count_EmptyInner_IsZero() => Assert.Equal(0, Wrap().Count);
}
