using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MaximumDepthOfBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumDepthOfBinaryTree;

// Harness only. Both strategies are MaximumDepthOfBinaryTreeSolution's - this
// file pins them to LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape (BinaryTreeNode<int> is internal, so it
// cannot appear in a public TheoryData signature; BuildTree reconstructs it).
public sealed class MaximumDepthOfBinaryTreeTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [3, 9, 20, null, null, 15, 7], 3 },
            { [1, null, 2], 2 },
            { [], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthByRecursion_LeetCodeExamples_ReturnsHeight(int?[] values, int expected) =>
        Assert.Equal(expected, MaximumDepthOfBinaryTreeSolution.MaxDepthByRecursion(BuildTree(values)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthByTreeMetrics_LeetCodeExamples_ReturnsHeight(int?[] values, int expected) =>
        Assert.Equal(expected, MaximumDepthOfBinaryTreeSolution.MaxDepthByTreeMetrics(BuildTree(values)));

    // LeetCode's level-order array shape: each existing node consumes exactly
    // two subsequent slots for its children, null marking a missing one.
    private static BinaryTreeNode<int>? BuildTree(int?[] values)
    {
        if (values.Length == 0 || values[0] is null)
        {
            return null;
        }

        var root = new BinaryTreeNode<int>(values[0]!.Value);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);
        var i = 1;

        while (queue.Count > 0 && i < values.Length)
        {
            var node = queue.Dequeue();

            if (values[i] is int leftValue)
            {
                node.Left = new BinaryTreeNode<int>(leftValue);
                queue.Enqueue(node.Left);
            }

            i++;

            if (i < values.Length && values[i] is int rightValue)
            {
                node.Right = new BinaryTreeNode<int>(rightValue);
                queue.Enqueue(node.Right);
            }

            i++;
        }

        return root;
    }
}
