using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeRightSideView;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeRightSideView;

// Harness only. The level-grouped BFS is BinaryTreeRightSideViewSolution's -
// this file pins it to LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape (BinaryTreeNode<int> is internal, so it
// cannot appear in a public TheoryData signature; BuildTree reconstructs it).
public sealed class BinaryTreeRightSideViewTests
{
    public static TheoryData<int?[], List<int>> Examples =>
        new()
        {
            { [1, 2, 3, null, 5, null, 4], [1, 3, 4] },
            { [1, null, 3], [1, 3] },
            { [], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RightSideViewByLevelGroupedTraversal_LeetCodeExamples_ReturnsRightmostPerLevel(
        int?[] values, List<int> expected) =>
        Assert.Equal(expected, BinaryTreeRightSideViewSolution.RightSideViewByLevelGroupedTraversal(BuildTree(values)));

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
