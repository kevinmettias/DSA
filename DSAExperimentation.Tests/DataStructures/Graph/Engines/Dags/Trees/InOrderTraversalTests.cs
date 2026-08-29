using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class InOrderTraversalTests
{
    private struct SampleMarker;
    private struct RightSkewedMarker;
    private struct LeftSkewedMarker;
    private struct SingleNodeMarker;
    private struct NullRootMarker;

    [Fact]
    public void Walk_VisitsLeftNodeRight()
    {
        var root = BinaryTreeTrees.Sample();

        InOrderTraversal.Walk<int, RecordingInOrderHooks<int, SampleMarker>>(root);

        Assert.Equal(
            new[] { 1, 2, 3, 4, 6, 7 },
            RecordingInOrderHooks<int, SampleMarker>.Visited.Select(v => v.Value));
    }

    // The regression test for the bug a naive "child 0 vs. the rest" n-ary
    // generalization would have: a Right-only node must fire before its right
    // subtree, not after.
    [Fact]
    public void Walk_RightOnlyNode_VisitsNodeBeforeRightSubtree()
    {
        var root = BinaryTreeTrees.RightSkewedPair();

        InOrderTraversal.Walk<int, RecordingInOrderHooks<int, RightSkewedMarker>>(root);

        Assert.Equal(
            new[] { 2, 3 },
            RecordingInOrderHooks<int, RightSkewedMarker>.Visited.Select(v => v.Value));
    }

    [Fact]
    public void Walk_LeftOnlyNode_VisitsLeftSubtreeBeforeNode()
    {
        var root = BinaryTreeTrees.LeftSkewedPair();

        InOrderTraversal.Walk<int, RecordingInOrderHooks<int, LeftSkewedMarker>>(root);

        Assert.Equal(
            new[] { 4, 5 },
            RecordingInOrderHooks<int, LeftSkewedMarker>.Visited.Select(v => v.Value));
    }

    [Fact]
    public void Walk_SingleNode_VisitsJustTheRoot()
    {
        var root = BinaryTreeTrees.SingleNode();

        InOrderTraversal.Walk<int, RecordingInOrderHooks<int, SingleNodeMarker>>(root);

        Assert.Equal(
            new[] { 1 },
            RecordingInOrderHooks<int, SingleNodeMarker>.Visited.Select(v => v.Value));
    }

    [Fact]
    public void Walk_NullRoot_NoVisits()
    {
        InOrderTraversal.Walk<int, RecordingInOrderHooks<int, NullRootMarker>>(null);

        Assert.Empty(RecordingInOrderHooks<int, NullRootMarker>.Visited);
    }
}
