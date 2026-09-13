using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.AllPossibleFullBinaryTrees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllPossibleFullBinaryTrees;

// Harness only: both strategies live in AllPossibleFullBinaryTreesSolution. The
// memoized arm was previously benchmark-only and unasserted - it shares subtree
// objects across parent splits, so the odd counts here pin both the Catalan result
// count and that every returned tree is still full with exactly n nodes.
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
    public void AllPossibleFbtByPlainRecursion_LeetCodeExamples_ReturnsEveryFullBinaryTreeOfThatSize(
        int n, int expectedCount) =>
        AssertAllFullWithNodeCount(
            AllPossibleFullBinaryTreesSolution.AllPossibleFbtByPlainRecursion(n), n, expectedCount);

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllPossibleFbtByMemoizedNodeCount_LeetCodeExamples_ReturnsEveryFullBinaryTreeOfThatSize(
        int n, int expectedCount) =>
        AssertAllFullWithNodeCount(
            AllPossibleFullBinaryTreesSolution.AllPossibleFbtByMemoizedNodeCount(n), n, expectedCount);

    [Fact]
    public void AllPossibleFbtByPlainRecursion_SingleNode_ReturnsOneLeaf() =>
        AssertSingleLeafTree(AllPossibleFullBinaryTreesSolution.AllPossibleFbtByPlainRecursion(1));

    [Fact]
    public void AllPossibleFbtByMemoizedNodeCount_SingleNode_ReturnsOneLeaf() =>
        AssertSingleLeafTree(AllPossibleFullBinaryTreesSolution.AllPossibleFbtByMemoizedNodeCount(1));

    private static void AssertAllFullWithNodeCount(
        List<BinaryTreeNode<int>?> trees, int n, int expectedCount)
    {
        Assert.Equal(expectedCount, trees.Count);
        Assert.All(trees, tree =>
        {
            Assert.Equal(n, CountNodes(tree));
            Assert.True(IsFull(tree));
        });
    }

    private static void AssertSingleLeafTree(List<BinaryTreeNode<int>?> trees)
    {
        var tree = Assert.Single(trees);
        Assert.Null(tree!.Left);
        Assert.Null(tree.Right);
    }

    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);

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
