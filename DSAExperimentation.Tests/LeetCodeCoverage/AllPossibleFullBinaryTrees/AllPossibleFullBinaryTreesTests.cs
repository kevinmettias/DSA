using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllPossibleFullBinaryTrees;

// LeetCode 894. All Possible Full Binary Trees: a full binary tree (every node has 0
// or 2 children) with n nodes splits into a 1-node root plus a left subtree of i
// nodes and a right subtree of (n-1-i) nodes for every odd i - every combination of
// a full binary tree generated for i and one generated for (n-1-i) forms one
// distinct result, built directly out of this repo's own BinaryTreeNode<int>, the
// same "every combination of a left/right generation" composition
// UniqueBinarySearchTreesIITests already uses for LeetCode 95.
public sealed partial class AllPossibleFullBinaryTreesTests
{
    [Fact]
    public void AllPossibleFbt_SevenNodes_ReturnsAllFiveDistinctFullBinaryTrees()
    {
        var trees = AllPossibleFbt(7);

        Assert.Equal(5, trees.Count);
        Assert.All(trees, tree =>
        {
            var isFullWithNodeCount = IsFullWithNodeCount(tree, 7);
            Assert.True(isFullWithNodeCount);
        });
    }

    [Fact]
    public void AllPossibleFbt_SingleNode_ReturnsOneLeaf()
    {
        var trees = AllPossibleFbt(1);

        var tree = Assert.Single(trees);
        Assert.Null(tree!.Left);
        Assert.Null(tree.Right);
    }

    [Fact]
    public void AllPossibleFbt_EvenNodeCount_ReturnsNoTrees()
    {
        var trees = AllPossibleFbt(4);

        Assert.Empty(trees);
    }

    private static List<BinaryTreeNode<int>?> AllPossibleFbt(int n)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (n % 2 == 0)
        {
            return trees;
        }

        if (n == 1)
        {
            trees.Add(new BinaryTreeNode<int>(0));
            return trees;
        }

        AddAllCombinations(trees, n);

        return trees;
    }

    private static void AddAllCombinations(List<BinaryTreeNode<int>?> trees, int n)
    {
        for (var leftCount = 1; leftCount < n; leftCount += 2)
        {
            var lefts = AllPossibleFbt(leftCount);
            var rights = AllPossibleFbt(n - 1 - leftCount);

            foreach (var left in lefts)
            {
                foreach (var right in rights)
                {
                    trees.Add(new BinaryTreeNode<int>(0) { Left = left, Right = right });
                }
            }
        }
    }

    private static bool IsFullWithNodeCount(BinaryTreeNode<int>? node, int expectedCount) =>
        CountNodes(node) == expectedCount && IsFull(node);

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
