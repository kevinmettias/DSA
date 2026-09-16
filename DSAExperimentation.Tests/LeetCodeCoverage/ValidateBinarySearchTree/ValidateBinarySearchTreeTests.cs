using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ValidateBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateBinarySearchTree;

// Harness only. The bounds-recursion validation itself is
// ValidateBinarySearchTreeSolution's - this file just pins it to LeetCode's
// published examples, given in LeetCode's own level-order-with-null array shape,
// plus the empty-tree edge case the original test never exercised.
// BinaryTreeNode<int> is internal, so - as in UniqueBinarySearchTreesIITests - it
// stays out of a public TheoryData signature and BuildTree reconstructs it from
// that array.
public sealed class ValidateBinarySearchTreeTests
{
    public static TheoryData<TreeExample> Examples =>
        new()
        {
            { new TreeExample(Values: [2, 1, 3], Expected: true) },
            { new TreeExample(Values: [5, 1, 4, null, null, 3, 6], Expected: false) },
            { new TreeExample(Values: [], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByBoundsRecursion_Examples_ReturnsWhetherEveryNodeStaysWithinItsBounds(TreeExample example)
    {
        var isValid = ValidateBinarySearchTreeSolution.IsValidByBoundsRecursion(BuildTree(example.Values));

        Assert.Equal(example.Expected, isValid);
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

    // One example: the tree in LeetCode's level-order-with-null array shape and
    // whether the bounds recursion should accept it as a binary search tree.
    public readonly record struct TreeExample(int?[] Values, bool Expected);
}
