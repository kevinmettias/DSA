using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class InOrderTraversalTests
{
    // BinaryTreeTrees.Sample's node count.
    private const int SampleNodeCount = 6;

    [Fact]
    public void Walk_VisitsLeftNodeRight() => Assert.Equal(new[] { 1, 2, 3, 4, 6, 7 }, InOrderValues.Of(BinaryTreeTrees.Sample()));

    // The regression test for the bug a naive "child 0 vs. the rest" n-ary
    // generalization would have: a Right-only node must fire before its right
    // subtree, not after.
    [Fact]
    public void Walk_RightOnlyNode_VisitsNodeBeforeRightSubtree() =>
        Assert.Equal(new[] { 2, 3 }, InOrderValues.Of(BinaryTreeTrees.RightSkewedPair()));

    [Fact]
    public void Walk_LeftOnlyNode_VisitsLeftSubtreeBeforeNode() =>
        Assert.Equal(new[] { 4, 5 }, InOrderValues.Of(BinaryTreeTrees.LeftSkewedPair()));

    [Fact]
    public void Walk_SingleNode_VisitsJustTheRoot() => Assert.Equal(new[] { 1 }, InOrderValues.Of(BinaryTreeTrees.SingleNode()));

    [Fact]
    public void Walk_NullRoot_NoVisits() => Assert.Empty(InOrderValues.Of(null));

    // Each visit has to act on the value Walk returns, not on a copy a recursive frame was given:
    // a hook passed down by value would come back having counted only the root.
    [Fact]
    public void Walk_ReturnsTheHookValueEveryVisitActedOn() =>
        Assert.Equal(SampleNodeCount, InOrderTraversal.Walk(BinaryTreeTrees.Sample(), new CountingInOrderHooks()).Visited);

    [Fact]
    public void Walk_NullRoot_ReturnsTheHookUnchanged() =>
        Assert.Equal(0, InOrderTraversal.Walk<int, CountingInOrderHooks>(null, new CountingInOrderHooks()).Visited);
}
