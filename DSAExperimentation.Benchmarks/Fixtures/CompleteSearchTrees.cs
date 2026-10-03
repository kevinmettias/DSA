using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Fixtures;

// A complete binary tree - the shape BinaryTrees.Complete lays out - whose node values are
// their in-order ranks 0..n-1, so it is also a binary search tree. BinaryTrees.Balanced has
// the same shape but labels nodes in level order, which makes it a heap rather than a BST;
// a problem whose LeetCode input is promised to be a BST is built from this instead.
internal static class CompleteSearchTrees
{
    public static BinaryTreeNode<int> InOrderRanked(int nodeCount) =>
        BinaryTrees.Complete(InOrderRankedLevelOrder(nodeCount));

    // The level-order array of the complete tree whose in-order walk reads 0, 1, ..., nodeCount - 1.
    private static int[] InOrderRankedLevelOrder(int nodeCount)
    {
        var levelOrder = new int[nodeCount];
        RankSubtree(levelOrder, index: 0, firstRank: 0);
        return levelOrder;
    }

    // Gives the subtree at heap index `index` (children at 2i + 1 and 2i + 2) the consecutive ranks
    // from firstRank on, left subtree first, and returns the rank after the last one it used.
    private static int RankSubtree(int[] levelOrder, int index, int firstRank)
    {
        if (index >= levelOrder.Length)
        {
            return firstRank;
        }

        var leftChild = (AlgorithmConstants.BranchingFactor * index) + 1;
        var ownRank = RankSubtree(levelOrder, leftChild, firstRank);
        levelOrder[index] = ownRank;
        return RankSubtree(levelOrder, leftChild + 1, ownRank + 1);
    }
}
