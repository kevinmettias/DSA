using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

// Evaluate is called directly rather than through TreeFold.Fold, which only adds the
// null-root check in front of it.
public sealed partial class RecursiveFoldEvaluationTests
{
    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Evaluate_CombinesEachNodeWithItsChildrenInChildOrder()
        => Assert.Equal("ABEFCDG", Evaluate(TestTrees.NArySample(), []));

    [Fact]
    public void Evaluate_ReversedChildOrder_ReachesCombineReversed()
        => Assert.Equal("ADGCBFE", EvaluateReversed(TestTrees.NArySample(), []));

    [Fact]
    public void Evaluate_CombinesInPostOrder()
    {
        var combined = new List<string>();

        Evaluate(TestTrees.NArySample(), combined);

        Assert.Equal(["E", "F", "B", "C", "G", "D", "A"], combined);
    }

    [Fact]
    public void Evaluate_SingleNode_CombinesItWithNoChildren()
        => Assert.Equal("A", Evaluate(TestTrees.SingleNode(), []));

    private static string Evaluate(TestNode root, List<string> combined)
        => RecursiveFoldEvaluation<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingNamesFoldAlgebra, string>(root, new RecordingNamesFoldAlgebra(combined));

    private static string EvaluateReversed(TestNode root, List<string> combined)
        => RecursiveFoldEvaluation<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            RecordingNamesFoldAlgebra, string>(root, new RecordingNamesFoldAlgebra(combined));
}
