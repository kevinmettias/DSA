using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversalII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeLevelOrderTraversalII;

// Harness only. Both search strategies are
// BinaryTreeLevelOrderTraversalIISolution's - this file just pins them to
// LeetCode's published examples, given in LeetCode's own level-order array
// notation (null marking a missing child) since BinaryTreeNode is internal and
// cannot appear in a public TheoryData signature. Building the tree from that
// array is harness plumbing, not part of either strategy under test - both
// strategies already take a prebuilt root, exactly like LeetCode's own
// TreeNode-typed signature.
public sealed class BinaryTreeLevelOrderTraversalIITests
{
    public static TheoryData<int?[], List<List<int>>> Examples =>
        new()
        {
            { [3, 9, 20, null, null, 15, 7], [[15, 7], [9, 20], [3]] },
            { [1], [[1]] },
            { [], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LevelOrderBottomByQueue_LeetCodeExamples_ReturnsLevelsBottomUp(
        int?[] levelOrder, List<List<int>> expected) =>
        Assert.Equal(
            expected,
            BinaryTreeLevelOrderTraversalIISolution.LevelOrderBottomByQueue(BuildTree(levelOrder)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LevelOrderBottomByLevelGroupedTraversal_LeetCodeExamples_ReturnsLevelsBottomUp(
        int?[] levelOrder, List<List<int>> expected) =>
        Assert.Equal(
            expected,
            BinaryTreeLevelOrderTraversalIISolution.LevelOrderBottomByLevelGroupedTraversal(BuildTree(levelOrder)));

    // Deserializes LeetCode's level-order array notation into this repo's
    // BinaryTreeNode. Harness input translation, not an algorithm strategy.
    private static BinaryTreeNode<int>? BuildTree(int?[] levelOrder)
    {
        if (levelOrder.Length == 0 || levelOrder[0] is not int rootValue)
        {
            return null;
        }

        var root = new BinaryTreeNode<int>(rootValue);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);
        var i = 1;

        while (queue.Count > 0 && i < levelOrder.Length)
        {
            var node = queue.Dequeue();

            if (i < levelOrder.Length && levelOrder[i++] is int leftValue)
            {
                node.Left = new BinaryTreeNode<int>(leftValue);
                queue.Enqueue(node.Left);
            }

            if (i < levelOrder.Length && levelOrder[i++] is int rightValue)
            {
                node.Right = new BinaryTreeNode<int>(rightValue);
                queue.Enqueue(node.Right);
            }
        }

        return root;
    }
}
