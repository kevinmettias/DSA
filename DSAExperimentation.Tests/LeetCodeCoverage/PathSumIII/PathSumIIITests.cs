using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSumIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSumIII;

// Harness only. Both strategies are PathSumIIISolution's - this file pins them
// to LeetCode's published examples, given in LeetCode's own level-order-with-
// null array shape (BinaryTreeNode<int> is internal, so it cannot appear in a
// public TheoryData signature; BuildTree reconstructs it).
public sealed class PathSumIIITests
{
    public static TheoryData<int?[], int, int> Examples =>
        new()
        {
            // 10
            // |-- 5
            // |   |-- 3
            // |   |   |-- 3
            // |   |   `-- -2
            // |   `-- 2
            // |       `-- 1
            // `-- -3
            //     `-- 11
            // Matching paths (sum 8): 5->3, 5->2->1, -3->11.
            { [10, 5, -3, 3, 2, null, 11, 3, -2, null, 1], 8, 3 },
            { [1, -2, -3], 100, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PathSumByDoubleDfs_LeetCodeExamples_ReturnsMatchingPathCount(
        int?[] values, int target, int expected)
    {
        var pathCount = PathSumIIISolution.PathSumByDoubleDfs(BuildTree(values), target);

        Assert.Equal(expected, pathCount);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PathSumByPrefixSumHashMap_LeetCodeExamples_ReturnsMatchingPathCount(
        int?[] values, int target, int expected)
    {
        var pathCount = PathSumIIISolution.PathSumByPrefixSumHashMap(BuildTree(values), target);

        Assert.Equal(expected, pathCount);
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
            var node = queue.Dequeue();
            i = AttachChildren(node, values, i, queue);
        }

        return root;
    }

    // Takes the two slots a dequeued node's children occupy, attaching each one that
    // exists and queueing it up, and returns the index of the next unattached slot.
    private static int AttachChildren(
        BinaryTreeNode<int> node, int?[] values, int i, Queue<BinaryTreeNode<int>> queue)
    {
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
