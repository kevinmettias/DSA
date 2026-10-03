using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

// Evaluate is called directly rather than through TreeFold.Fold, which only adds the
// null-root check in front of it.
public sealed partial class RecursiveFoldEvaluationTests
{
    private struct NaturalOrderMarker;
    private struct ReversedOrderMarker;
    private struct EnterMarker;
    private struct SingleNodeMarker;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Evaluate_CombinesEachNodeWithItsChildrenInChildOrder()
        => Assert.Equal("ABEFCDG", Evaluate<NaturalOrderMarker>(TestTrees.NArySample()));

    [Fact]
    public void Evaluate_ReversedChildOrder_ReachesCombineReversed()
        => Assert.Equal("ADGCBFE", EvaluateReversed<ReversedOrderMarker>(TestTrees.NArySample()));

    [Fact]
    public void Evaluate_EntersDepthFirstWithEachNodesDepth()
    {
        Evaluate<EnterMarker>(TestTrees.NArySample());

        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("E", 2), ("F", 2), ("C", 1), ("D", 1), ("G", 2) },
            RecordingNamesFoldAlgebra<EnterMarker>.Entered);
    }

    [Fact]
    public void Evaluate_SingleNode_CombinesItWithNoChildren()
        => Assert.Equal("A", Evaluate<SingleNodeMarker>(TestTrees.SingleNode()));

    private static string Evaluate<TMarker>(TestNode root)
        where TMarker : struct
        => RecursiveFoldEvaluation<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingNamesFoldAlgebra<TMarker>, string>(root);

    private static string EvaluateReversed<TMarker>(TestNode root)
        where TMarker : struct
        => RecursiveFoldEvaluation<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            RecordingNamesFoldAlgebra<TMarker>, string>(root);
}
