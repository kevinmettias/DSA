using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.AllPossibleFullBinaryTrees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllPossibleFullBinaryTrees;

// Harness only: both strategies live in AllPossibleFullBinaryTreesSolution. The
// memoized arm was previously benchmark-only and unasserted - it shares subtree
// objects across parent splits, so the odd counts here pin both the Catalan result
// count and that every returned tree is still full with exactly nodeCount nodes.
public sealed class AllPossibleFullBinaryTreesTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 7, 5 },
            { 3, 1 },
            { 1, 1 },
            { 5, 2 },
            { 9, 14 },
            { 2, 0 },
            { 4, 0 },
            { 6, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllPossibleFullBinaryTreesByPlainRecursion_LeetCodeExamples_ReturnsEveryFullBinaryTreeOfThatSize(
        int nodeCount, int expectedCount) =>
        AssertAllFullWithNodeCount(
            AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByPlainRecursion(nodeCount), nodeCount, expectedCount);

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllPossibleFullBinaryTreesByMemoizedNodeCount_LeetCodeExamples_ReturnsEveryFullBinaryTreeOfThatSize(
        int nodeCount, int expectedCount) =>
        AssertAllFullWithNodeCount(
            AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByMemoizedNodeCount(nodeCount), nodeCount, expectedCount);

    [Fact]
    public void AllPossibleFullBinaryTreesByPlainRecursion_SingleNode_ReturnsOneLeaf() =>
        AssertSingleLeafTree(AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByPlainRecursion(1));

    [Fact]
    public void AllPossibleFullBinaryTreesByMemoizedNodeCount_SingleNode_ReturnsOneLeaf() =>
        AssertSingleLeafTree(AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByMemoizedNodeCount(1));

    private static void AssertAllFullWithNodeCount(
        List<BinaryTreeNode<int>?> trees, int nodeCount, int expectedCount)
    {
        Assert.Equal(expectedCount, trees.Count);
        Assert.All(trees, tree =>
        {
            Assert.Equal(nodeCount, CountNodes(tree));
            Assert.True(IsFull(tree));
        });
    }

    private static void AssertSingleLeafTree(List<BinaryTreeNode<int>?> trees)
    {
        // nodeCount = 1 is the recurrence's base case, which lists the leaf it builds,
        // and the solution reports "no full binary tree of this size" as an empty list
        // (every even nodeCount above) rather than as a null entry - so the one entry
        // here is a tree.
        // IsType asks for it instead of promising it.
        var tree = Assert.IsType<BinaryTreeNode<int>>(Assert.Single(trees));

        Assert.Null(tree.Left);
        Assert.Null(tree.Right);
    }

    // Both arms are values: 0 for the empty subtree, and a call naming the non-empty
    // count. The non-empty arm stays a call rather than a hoisted local because the
    // condition guards it - a local above the expression would count the children of a
    // node that is not there.
    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : CountSubtreeNodes(node);

    private static int CountSubtreeNodes(BinaryTreeNode<int> node) =>
        1 + CountNodes(node.Left) + CountNodes(node.Right);

    private static bool IsFull(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return true;
        }

        var hasLeft = node.Left is not null;
        var hasRight = node.Right is not null;

        return hasLeft == hasRight && IsFull(node.Left) && IsFull(node.Right);
    }
}
