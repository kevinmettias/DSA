using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SearchInABinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInABinarySearchTree;

// Harness only. Both strategies are SearchInABinarySearchTreeSolution's; this
// file pins them to LeetCode's published examples, including the one where the
// value is absent and the one where the value is the root itself (its own
// subtree - the whole tree - comes back).
public sealed partial class SearchInABinarySearchTreeTests
{
    public static TheoryData<SubtreeSearchExample> Examples =>
        new()
        {
            new SubtreeSearchExample(
                TreeValues: [4, 2, 7, 1, 3], Value: 2, ExpectedValue: 2, ExpectedLeft: 1, ExpectedRight: 3),
            new SubtreeSearchExample(
                TreeValues: [4, 2, 7, 1, 3], Value: 5, ExpectedValue: null, ExpectedLeft: null, ExpectedRight: null),
            new SubtreeSearchExample(
                TreeValues: [4, 2, 7, 1, 3], Value: 4, ExpectedValue: 4, ExpectedLeft: 2, ExpectedRight: 7),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchBstByLinearScan_LeetCodeExamples_ReturnsSubtreeRootedAtValue(
        SubtreeSearchExample example)
    {
        var root = BuildTree(example.TreeValues);

        var found = SearchInABinarySearchTreeSolution.SearchBstByLinearScan(root, example.Value);

        AssertFound(found, example);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchBstByBstDescent_LeetCodeExamples_ReturnsSubtreeRootedAtValue(
        SubtreeSearchExample example)
    {
        var root = BuildTree(example.TreeValues);

        var found = SearchInABinarySearchTreeSolution.SearchBstByBstDescent(root, example.Value);

        AssertFound(found, example);
    }

    private static void AssertFound(BinaryTreeNode<int>? found, SubtreeSearchExample example)
    {
        if (example.ExpectedValue is null)
        {
            Assert.Null(found);
            return;
        }

        // A non-null ExpectedValue above means this row searches for a value the tree
        // holds - every example pairs the two - and both strategies return null only
        // for a value the tree does not hold. IsType asks for that node rather than
        // promising it, and pins the answer to the tree's own node type while there.
        var subtree = Assert.IsType<BinaryTreeNode<int>>(found);

        Assert.Equal(example.ExpectedValue, subtree.Value);
        Assert.Equal(example.ExpectedLeft, subtree.Left?.Value);
        Assert.Equal(example.ExpectedRight, subtree.Right?.Value);
    }

    private static BinaryTreeNode<int> BuildTree(params int[] values)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        // Every call site in this file passes a non-empty values array, so at least
        // one Insert above always ran and Root is never null here.
        return tree.Root
            ?? throw new InvalidOperationException(
                "every call site passes a non-empty values array, so the first Insert above set Root");
    }

    // One LeetCode example: the values to insert, the value to search for, and the
    // subtree that must come back - its own value and its two children (all null
    // when the value is absent). The five travel together as one case, so the
    // signature carries one parameter rather than five positions.
    public readonly record struct SubtreeSearchExample(
        int[] TreeValues,
        int Value,
        int? ExpectedValue,
        int? ExpectedLeft,
        int? ExpectedRight);
}
