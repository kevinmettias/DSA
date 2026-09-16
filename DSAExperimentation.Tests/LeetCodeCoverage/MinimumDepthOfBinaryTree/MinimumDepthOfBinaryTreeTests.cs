using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MinimumDepthOfBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumDepthOfBinaryTree;

// Harness only. The single recursive strategy lives in
// MinimumDepthOfBinaryTreeSolution and is asserted against LeetCode's published
// examples, the original test's right-skewed case, and the empty-tree edge case
// none of the pre-migration files exercised. BinaryTreeNode<int> is internal, so -
// as in SameTreeTests - it stays out of a public TheoryData signature and
// BuildTree reconstructs it from LeetCode's own level-order-with-null array shape.
public sealed class MinimumDepthOfBinaryTreeTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [], 0 },
            // The original test's right-skewed case, root = [1,2,3,null,null,null,4] -> 2:
            // the root's left child is the only shallow leaf.
            { [1, 2, 3, null, null, null, 4], 2 },
            // LeetCode 111's own example: root = [3,9,20,null,null,15,7] -> 2.
            { [3, 9, 20, null, null, 15, 7], 2 },
            // LeetCode 111's own example: root = [2,null,3,null,4,null,5,null,6] -> 5.
            // A leaf is only reachable by descending every right child, so the minimum
            // depth equals the maximum depth here.
            { [2, null, 3, null, 4, null, 5, null, 6], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDepthByRecursion_Examples_ReturnsShortestLeafDepth(int?[] values, int expectedDepth)
    {
        var depth = MinimumDepthOfBinaryTreeSolution.MinDepthByRecursion(BuildTree(values));

        Assert.Equal(expectedDepth, depth);
    }

    // LeetCode's level-order array shape: each existing node consumes exactly
    // two subsequent slots for its children, null marking a missing one.
    private static BinaryTreeNode<int>? BuildTree(int?[] values)
    {
        if (values.Length == 0 || values[0] is null)
        {
            return null;
        }

        var root = new BinaryTreeNode<int>(values[0].Value);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);

        var i = 1;
        while (queue.Count > 0 && i < values.Length)
        {
            i = AttachChildren(values, i, queue);
        }

        return root;
    }

    // Consumes one slot for each of the dequeued parent's children and returns the
    // index just past them: a null or absent slot attaches nothing but is still spent.
    private static int AttachChildren(int?[] values, int i, Queue<BinaryTreeNode<int>> queue)
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

        return i + 1;
    }
}
