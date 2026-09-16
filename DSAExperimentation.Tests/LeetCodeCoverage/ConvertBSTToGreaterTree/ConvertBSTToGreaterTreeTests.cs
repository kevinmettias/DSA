using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConvertBSTToGreaterTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertBSTToGreaterTree;

// Harness only: both strategies live in ConvertBSTToGreaterTreeSolution and are
// asserted against LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape. BinaryTreeNode<int> is internal, so - as in
// BinaryTreeLevelOrderTraversalTests - it stays out of a public TheoryData signature
// and BuildTree reconstructs both the input tree and the expected one from that
// shape (CS0053 is why this file used one [Fact] per example before).
public sealed class ConvertBSTToGreaterTreeTests
{
    public static TheoryData<TreeExample> Examples =>
        new()
        {
            // [0, null, 1] -> [1, null, 1]
            { new TreeExample([0, null, 1], [1, null, 1]) },

            //       5                  29
            //      / \                /  \
            //     3   8      ->     36    17
            //    / \ / \            / \   / \
            //   2  4 7  9         38 33  24  9
            { new TreeExample([5, 3, 8, 2, 4, 7, 9], [29, 36, 17, 38, 33, 24, 9]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConvertByReverseInOrder_LeetCodeExamples_AccumulatesSumOfGreaterValues(TreeExample example)
    {
        var actual = ConvertBSTToGreaterTreeSolution.ConvertByReverseInOrder(BuildTree(example.Values));

        AssertTreeEqual(BuildTree(example.Expected), actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConvertByInOrderHooks_LeetCodeExamples_AccumulatesSumOfGreaterValues(TreeExample example)
    {
        var actual = ConvertBSTToGreaterTreeSolution.ConvertByInOrderHooks(BuildTree(example.Values));

        AssertTreeEqual(BuildTree(example.Expected), actual);
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

    private static void AssertTreeEqual(BinaryTreeNode<int>? expected, BinaryTreeNode<int>? actual)
    {
        if (expected is null || actual is null)
        {
            // The two trees must end together, and a tree that ends where the other
            // continues is the shape mismatch this recursion reports. One guard covers
            // both sides, so nothing here has to promise a node is present.
            Assert.Equal(expected is null, actual is null);
            return;
        }

        Assert.Equal(expected.Value, actual.Value);
        AssertTreeEqual(expected.Left, actual.Left);
        AssertTreeEqual(expected.Right, actual.Right);
    }

    public readonly record struct TreeExample(int?[] Values, int?[] Expected);
}
