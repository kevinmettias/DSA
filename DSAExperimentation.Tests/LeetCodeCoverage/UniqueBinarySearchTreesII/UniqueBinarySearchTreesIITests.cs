using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniqueBinarySearchTreesII;

// LeetCode 95. Unique Binary Search Trees II: for each candidate root k in
// [start,end], every combination of a left subtree generated from [start,k-1] and
// a right subtree generated from [k+1,end] forms one distinct BST - built directly
// out of this repo's own BinaryTreeNode<int>, the same "construct nodes ad hoc via
// object initializers" style ConvertSortedArrayToBinarySearchTree already uses,
// rather than UniqueBinarySearchTrees's Insert-based BinarySearchTree<TValue>
// (insertion order only ever produces ONE shape per order, not every shape).
public sealed partial class UniqueBinarySearchTreesIITests
{
    [Fact]
    public void GenerateTrees_NThree_ReturnsAllFiveStructurallyValidBsts()
    {
        var trees = GenerateTrees(1, 3);

        Assert.Equal(5, trees.Count);
        Assert.All(trees, tree => Assert.Equal([1, 2, 3], InOrder(tree)));
    }

    [Fact]
    public void GenerateTrees_NOne_ReturnsSingleLeafTree()
    {
        var trees = GenerateTrees(1, 1);

        var tree = Assert.Single(trees);
        Assert.Equal(1, tree!.Value);
        Assert.Null(tree.Left);
        Assert.Null(tree.Right);
    }

    private static List<BinaryTreeNode<int>?> GenerateTrees(int start, int end)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (start > end)
        {
            trees.Add(null);
            return trees;
        }

        for (var root = start; root <= end; root++)
        {
            var lefts = GenerateTrees(start, root - 1);
            var rights = GenerateTrees(root + 1, end);

            foreach (var left in lefts)
            {
                foreach (var right in rights)
                {
                    trees.Add(new BinaryTreeNode<int>(root) { Left = left, Right = right });
                }
            }
        }

        return trees;
    }

    private static int[] InOrder(BinaryTreeNode<int>? node) =>
        node is null ? [] : [.. InOrder(node.Left), node.Value, .. InOrder(node.Right)];
}
