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
    private struct CombineOrderMarker;
    private struct SingleNodeMarker;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Evaluate_CombinesEachNodeWithItsChildrenInChildOrder()
        => Assert.Equal("ABEFCDG", Evaluate<NaturalOrderMarker>(TestTrees.NArySample()));

    [Fact]
    public void Evaluate_ReversedChildOrder_ReachesCombineReversed()
        => Assert.Equal("ADGCBFE", EvaluateReversed<ReversedOrderMarker>(TestTrees.NArySample()));

    [Fact]
    public void Evaluate_CombinesInPostOrder()
    {
        Evaluate<CombineOrderMarker>(TestTrees.NArySample());

        Assert.Equal(["E", "F", "B", "C", "G", "D", "A"], RecordingNamesFoldAlgebra<CombineOrderMarker>.Combined);
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
