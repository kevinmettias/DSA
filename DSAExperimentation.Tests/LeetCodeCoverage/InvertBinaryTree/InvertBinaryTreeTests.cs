using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InvertBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InvertBinaryTree;

// Harness only. The single strategy is InvertBinaryTreeSolution's; this file pins
// it to LeetCode's published examples plus the null/leaf edges the original test
// never exercised. BinaryTreeNode<int> is internal, so - as in SameTreeTests - it
// stays out of a public TheoryData/[Theory] signature and BuildTree reconstructs
// it from LeetCode's own level-order-with-null array shape.
public sealed class InvertBinaryTreeTests
{
    [Fact]
    public void InvertByRecursiveSwap_LeetCodeExampleOne_MirrorsEveryLevel()
    {
        // [4,2,7,1,3,6,9] -> [4,7,2,9,6,3,1]
        var inverted = Assert.IsType<BinaryTreeNode<int>>(
            InvertBinaryTreeSolution.InvertByRecursiveSwap(BuildTree([4, 2, 7, 1, 3, 6, 9])));

        AssertNode(inverted, value: 4, left: 7, right: 2);
        AssertNode(inverted.Left, value: 7, left: 9, right: 6);
        AssertNode(inverted.Right, value: 2, left: 3, right: 1);
    }

    [Fact]
    public void InvertByRecursiveSwap_LeetCodeExampleTwo_SwapsTheOnlyPairOfChildren()
    {
        // [2,1,3] -> [2,3,1]
        var inverted = Assert.IsType<BinaryTreeNode<int>>(
            InvertBinaryTreeSolution.InvertByRecursiveSwap(BuildTree([2, 1, 3])));

        AssertNode(inverted, value: 2, left: 3, right: 1);
    }

    [Fact]
    public void InvertByRecursiveSwap_EmptyTree_ReturnsNull() =>
        Assert.Null(InvertBinaryTreeSolution.InvertByRecursiveSwap(null));

    [Fact]
    public void InvertByRecursiveSwap_SingleNode_ReturnsItUnchanged()
    {
        var inverted = Assert.IsType<BinaryTreeNode<int>>(
            InvertBinaryTreeSolution.InvertByRecursiveSwap(BuildTree([1])));

        AssertNode(inverted, value: 1, left: null, right: null);
    }

    [Fact]
    public void InvertByRecursiveSwap_LeftOnlyChain_BecomesARightOnlyChain()
    {
        // [1,2,null,3]: 1's left child is 2, and 2's left child is 3.
        var inverted = Assert.IsType<BinaryTreeNode<int>>(
            InvertBinaryTreeSolution.InvertByRecursiveSwap(BuildTree([1, 2, null, 3])));

        AssertNode(inverted, value: 1, left: null, right: 2);
        AssertNode(inverted.Right, value: 2, left: null, right: 3);
    }

    // The value at a node together with the values of its two children, either of which
    // may be absent: that is what every example above checks about the node it is
    // looking at, stated once here instead of once per example. A child slot really can
    // be absent, so the node itself is asked for rather than promised - passing null
    // fails here with the value that was expected, not with a NullReferenceException.
    private static void AssertNode(BinaryTreeNode<int>? node, int value, int? left, int? right)
    {
        var present = Assert.IsType<BinaryTreeNode<int>>(node);

        Assert.Equal(value, present.Value);
        Assert.Equal(left, present.Left?.Value);
        Assert.Equal(right, present.Right?.Value);
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
