using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.DiameterOfBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DiameterOfBinaryTree;

// LeetCode 543. Diameter of Binary Tree. See DiameterOfBinaryTreeSolution for the
// two strategies: a naive recompute-height-per-node baseline, and this repo's own
// TreeMetrics.Diameter fold. Examples are stated as LeetCode's own level-order
// arrays - BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData member; BuildTree reconstructs it inside each test method instead.
public sealed class DiameterOfBinaryTreeTests
{
    public static TheoryData<int?[], int> Examples()
    {
        var data = new TheoryData<int?[], int>
        {
            { [1, 2, 3, 4, 5], 3 },
            { [1, 2], 1 },
            { [1], 0 },
        };

        return data;
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DiameterByRecomputedHeightPerNode_ReturnsLongestPathEdgeCount(int?[] levelOrder, int expected)
        => Assert.Equal(expected, DiameterOfBinaryTreeSolution.DiameterByRecomputedHeightPerNode(BuildTree(levelOrder)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DiameterByTreeMetricsFold_ReturnsLongestPathEdgeCount(int?[] levelOrder, int expected)
        => Assert.Equal(expected, DiameterOfBinaryTreeSolution.DiameterByTreeMetricsFold(BuildTree(levelOrder)));

    // LeetCode's own level-order input shape: a BFS-ordered array with null
    // standing in for a missing child.
    private static BinaryTreeNode<int> BuildTree(int?[] levelOrder)
    {
        var root = new BinaryTreeNode<int>(levelOrder[0]!.Value);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);
        var i = 1;

        while (i < levelOrder.Length)
        {
            var current = queue.Dequeue();

            if (i < levelOrder.Length && levelOrder[i] is { } leftValue)
            {
                current.Left = new BinaryTreeNode<int>(leftValue);
                queue.Enqueue(current.Left);
            }

            i++;

            if (i < levelOrder.Length && levelOrder[i] is { } rightValue)
            {
                current.Right = new BinaryTreeNode<int>(rightValue);
                queue.Enqueue(current.Right);
            }

            i++;
        }

        return root;
    }
}
