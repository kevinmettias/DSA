using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoSumIVInputIsABST;

// LeetCode 653. Two Sum IV - Input is a BST: build the tree with this repo's own
// BinarySearchTree<int>.Insert, then a single DFS over the resulting
// BinaryTreeNode<int> shape checks each node's complement against a Set<int> of
// values already seen - the same one-pass Set/HashMap idiom TwoSumTests/
// ContainsDuplicateTests already use, just walking tree edges instead of an array
// index.
public sealed partial class TwoSumIVInputIsABSTTests
{
    [Fact]
    public void FindTarget_PairSumsToTarget_ReturnsTrue()
    {
        var tree = BuildClassicExampleTree();

        Assert.True(FindTarget(tree.Root, k: 9));
    }

    [Fact]
    public void FindTarget_NoPairSumsToTarget_ReturnsFalse()
    {
        var tree = BuildClassicExampleTree();

        Assert.False(FindTarget(tree.Root, k: 28));
    }

    // Insert order [5,3,6,2,4,7] recreates the classic LC 653 example tree exactly:
    //         5
    //        / \
    //       3   6
    //      / \   \
    //     2   4   7
    private static BinarySearchTree<int> BuildClassicExampleTree()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in new[] { 5, 3, 6, 2, 4, 7 })
        {
            tree.Insert(value);
        }

        return tree;
    }

    private static bool FindTarget(BinaryTreeNode<int>? node, int k) => FindTarget(node, k, new Set<int>());

    private static bool FindTarget(BinaryTreeNode<int>? node, int k, Set<int> seen)
    {
        if (node is null)
        {
            return false;
        }

        if (seen.Has(k - node.Value))
        {
            return true;
        }

        seen.TryAdd(node.Value);
        return FindTarget(node.Left, k, seen) || FindTarget(node.Right, k, seen);
    }
}
