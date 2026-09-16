using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.UniqueBinarySearchTreesII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniqueBinarySearchTreesII;

// Harness only. Both strategies are UniqueBinarySearchTreesIISolution's - this
// file pins them to LeetCode's published example (n=3) and the n=1 edge case,
// checking every returned tree is a structurally valid BST over [1..n] via
// its in-order sequence, plus the n=1 case's exact single-leaf shape.
public sealed class UniqueBinarySearchTreesIITests
{
    public static TheoryData<int, int, int[]> Examples =>
        new()
        {
            { 3, 5, [1, 2, 3] },
            { 1, 1, [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateTreesByPlainRecursion_LeetCodeExamples_ReturnsAllStructurallyValidBsts(
        int nodeCount, int expectedCount, int[] expectedInOrder) =>
        AssertAllValidBsts(
            UniqueBinarySearchTreesIISolution.GenerateTreesByPlainRecursion(nodeCount), expectedCount, expectedInOrder);

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateTreesByMemoizedRange_LeetCodeExamples_ReturnsAllStructurallyValidBsts(
        int nodeCount, int expectedCount, int[] expectedInOrder) =>
        AssertAllValidBsts(
            UniqueBinarySearchTreesIISolution.GenerateTreesByMemoizedRange(nodeCount), expectedCount, expectedInOrder);

    [Fact]
    public void GenerateTreesByPlainRecursion_NOne_ReturnsSingleLeafTree() =>
        AssertSingleLeafTree(UniqueBinarySearchTreesIISolution.GenerateTreesByPlainRecursion(1));

    [Fact]
    public void GenerateTreesByMemoizedRange_NOne_ReturnsSingleLeafTree() =>
        AssertSingleLeafTree(UniqueBinarySearchTreesIISolution.GenerateTreesByMemoizedRange(1));

    private static void AssertAllValidBsts(List<BinaryTreeNode<int>?> trees, int expectedCount, int[] expectedInOrder)
    {
        Assert.Equal(expectedCount, trees.Count);
        Assert.All(trees, tree => Assert.Equal(expectedInOrder, InOrder(tree)));
    }

    private static void AssertSingleLeafTree(List<BinaryTreeNode<int>?> trees)
    {
        // For n = 1 the recurrence's only non-inverted branch is (1,1): the nulls it
        // lists for the inverted (1,0) and (2,1) become that leaf's absent children
        // rather than a list entry, since AppendCombinations fills the list with the
        // node it builds. IsType asks for that node instead of promising it is there.
        var tree = Assert.IsType<BinaryTreeNode<int>>(Assert.Single(trees));

        Assert.Equal(1, tree.Value);
        Assert.Null(tree.Left);
        Assert.Null(tree.Right);
    }

    private static int[] InOrder(BinaryTreeNode<int>? node) =>
        node is null ? Array.Empty<int>() : SubtreeSequence(node);

    private static int[] SubtreeSequence(BinaryTreeNode<int> node) =>
        [.. InOrder(node.Left), node.Value, .. InOrder(node.Right)];
}
