using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

// Evaluate is called directly rather than through TreeFold.Fold, which only adds the
// null-root check in front of it.
public sealed partial class IterativeFoldEvaluationTests
{
    // Far deeper than the default one-megabyte stack holds frames for, so a fold that
    // recursed once per level would overflow before reaching the bottom.
    private const int DeeperThanTheCallStack = 100_000;

    private struct NaturalOrderMarker;
    private struct ReversedOrderMarker;
    private struct EnterMarker;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    //
    // The bottom-up pass walks nodes in reverse discovery order, but each node's children
    // still reach Combine in the order the child-order witness gave them.
    [Fact]
    public void Evaluate_CombinesEachNodeWithItsChildrenInChildOrder()
        => Assert.Equal("ABEFCDG", Evaluate<NaturalOrderMarker>(TestTrees.NArySample()));

    [Fact]
    public void Evaluate_ReversedChildOrder_ReachesCombineReversed()
        => Assert.Equal("ADGCBFE", EvaluateReversed<ReversedOrderMarker>(TestTrees.NArySample()));

    // Discovery is breadth-first, so Enter sees the levels in turn - the one place an
    // impure algebra could tell this strategy from the recursive one.
    [Fact]
    public void Evaluate_EntersBreadthFirstWithEachNodesDepth()
    {
        Evaluate<EnterMarker>(TestTrees.NArySample());

        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("C", 1), ("D", 1), ("E", 2), ("F", 2), ("G", 2) },
            RecordingNamesFoldAlgebra<EnterMarker>.Entered);
    }

    [Fact]
    public void Evaluate_ChainDeeperThanTheCallStack_FoldsWithoutOverflowing()
    {
        var root = Chain(DeeperThanTheCallStack);

        var count = IterativeFoldEvaluation<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            CountNodesFoldAlgebra, int>(root);

        Assert.Equal(DeeperThanTheCallStack, count);
    }

    private static string Evaluate<TMarker>(TestNode root)
        where TMarker : struct
        => IterativeFoldEvaluation<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingNamesFoldAlgebra<TMarker>, string>(root);

    private static string EvaluateReversed<TMarker>(TestNode root)
        where TMarker : struct
        => IterativeFoldEvaluation<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            RecordingNamesFoldAlgebra<TMarker>, string>(root);

    // A single path of the given length: every node has exactly one child but the last.
    private static TestNode Chain(int length)
    {
        var root = new TestNode("0");
        var tail = root;

        for (var i = 1; i < length; i++)
        {
            var next = new TestNode(i.ToString());
            tail.Children.Add(next);
            tail = next;
        }

        return root;
    }
}
