using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MinimumAbsoluteDifferenceInBST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAbsoluteDifferenceInBST;

// Harness only. Both strategies are MinimumAbsoluteDifferenceInBSTSolution's - this
// file pins them to LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape. BinaryTreeNode<int> is internal, so - as in
// RecoverBinarySearchTreeTests - it stays out of a public TheoryData signature and
// BuildTree reconstructs it from that array.
public sealed class MinimumAbsoluteDifferenceInBSTTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            // [1,null,3,2] -> in-order 1,2,3 -> min diff 1
            { [1, null, 3, 2], 1 },
            // [4,2,6,1,3] -> in-order 1,2,3,4,6 -> min diff 1
            { [4, 2, 6, 1, 3], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMinimumDifferenceByRecursiveScan_LeetCodeExamples_ReturnsSmallestAdjacentGap(
        int?[] values, int expected)
    {
        var minimumDifference =
            MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByRecursiveScan(BuildTree(values));

        Assert.Equal(expected, minimumDifference);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMinimumDifferenceByInOrderHooks_LeetCodeExamples_ReturnsSmallestAdjacentGap(
        int?[] values, int expected)
    {
        var minimumDifference =
            MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByInOrderHooks(BuildTree(values));

        Assert.Equal(expected, minimumDifference);
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
