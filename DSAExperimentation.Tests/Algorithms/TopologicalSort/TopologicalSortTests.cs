using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;
using TopologicalSortOperations = DSAExperimentation.Algorithms.TopologicalSort.TopologicalSort;

namespace DSAExperimentation.Tests.Algorithms.TopologicalSort;

public sealed partial class TopologicalSortTests
{
    [Fact]
    public void TrySort_LinearChain_ReturnsOrderRespectingEdges()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.Add(b);
        b.Children.Add(c);

        var succeeded = TopologicalSortOperations.TrySort<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c], out var ordering);

        Assert.True(succeeded);
        Assert.Equal([a, b, c], ordering);
    }

    // A -> B, A -> C, B -> D, C -> D. Kahn's own FIFO frontier makes this order
    // deterministic: A drains first (the only initial root), which enqueues B then C in
    // that order, so B is dequeued - and D's in-degree decremented - before C is.
    [Fact]
    public void TrySort_DiamondDependency_RespectsAllEdges()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");
        a.Children.Add(b);
        a.Children.Add(c);
        b.Children.Add(d);
        c.Children.Add(d);

        var succeeded = TopologicalSortOperations.TrySort<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c, d], out var ordering);

        Assert.True(succeeded);
        Assert.Equal([a, b, c, d], ordering);
    }

    [Fact]
    public void TrySort_CycleAmongAllNodes_ReturnsFalse()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.Add(b);
        b.Children.Add(c);
        c.Children.Add(a);

        var succeeded = TopologicalSortOperations.TrySort<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c], out _);

        Assert.False(succeeded);
    }

    [Fact]
    public void TrySort_SelfLoop_ReturnsFalse()
    {
        var a = new TestNode("A");
        a.Children.Add(a);

        var succeeded = TopologicalSortOperations.TrySort<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a], out _);

        Assert.False(succeeded);
    }

    // nodes omits B even though A -> B exists. B still gets discovered via A's own edge,
    // which inflates the produced ordering past vertices.Count - the self-detecting
    // direction of the complete-vertex-set precondition.
    [Fact]
    public void TrySort_OmittedDescendant_ReturnsFalse()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        a.Children.Add(b);

        var succeeded = TopologicalSortOperations.TrySort<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a], out _);

        Assert.False(succeeded);
    }

    // nodes omits A even though A -> B and A -> C exist. Nothing forward-reachable from
    // {B, C} points back to A, so there is no way for TrySort to notice - the silent
    // direction of the same precondition, which a caller can't detect from the return
    // value alone.
    [Fact]
    public void TrySort_OmittedAncestor_ReturnsTrueButOmitsIt()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.Add(b);
        a.Children.Add(c);
        b.Children.Add(c);

        var succeeded = TopologicalSortOperations.TrySort<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [b, c], out var ordering);

        Assert.True(succeeded);
        Assert.Equal([b, c], ordering);
    }

    [Fact]
    public void TrySort_DuplicateNode_DoesNotAppearTwice()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        a.Children.Add(b);

        var succeeded = TopologicalSortOperations.TrySort<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, a, b], out var ordering);

        Assert.True(succeeded);
        Assert.Equal([a, b], ordering);
    }

    [Fact]
    public void TrySort_EmptyInput_ReturnsTrueWithEmptyOrdering()
    {
        var succeeded = TopologicalSortOperations.TrySort<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [], out var ordering);

        Assert.True(succeeded);
        Assert.Empty(ordering);
    }
}
