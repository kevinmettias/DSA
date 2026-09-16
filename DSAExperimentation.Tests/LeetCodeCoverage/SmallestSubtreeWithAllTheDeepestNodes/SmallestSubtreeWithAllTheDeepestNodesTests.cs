using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SmallestSubtreeWithAllTheDeepestNodes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestSubtreeWithAllTheDeepestNodes;

// Harness only. Both strategies are SmallestSubtreeWithAllTheDeepestNodesSolution's -
// the hand-rolled (depth, candidate) recursion and the TreeFold pass over
// DeepestSubtreeAlgebra. Examples are stated in LeetCode's own level-order-with-null
// array shape (BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData signature), and the expected answer is the value of the subtree root
// LeetCode reports - values are distinct in every example, so the value identifies
// exactly one node of the tree.
public sealed class SmallestSubtreeWithAllTheDeepestNodesTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [3, 5, 1, 6, 2, 0, 8, null, null, 7, 4], 2 },
            { [1], 1 },
            { [0, 1, 3, null, 2], 2 },
            { [1, 2, 3, 4, 5, 6, 7], 1 },
            { [0, 1, null, null, 3], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubtreeWithAllDeepestByRecursion_LeetCodeExamples_ReturnsSmallestSubtreeRoot(
        int?[] levelOrder, int expected) =>
        Assert.Equal(
            expected,
            SubtreeRoot(
                SmallestSubtreeWithAllTheDeepestNodesSolution.SubtreeWithAllDeepestByRecursion(BuildTree(levelOrder))).Value);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubtreeWithAllDeepestByTreeFold_LeetCodeExamples_ReturnsSmallestSubtreeRoot(
        int?[] levelOrder, int expected) =>
        Assert.Equal(
            expected,
            SubtreeRoot(
                SmallestSubtreeWithAllTheDeepestNodesSolution.SubtreeWithAllDeepestByTreeFold(BuildTree(levelOrder))).Value);

    // Both strategies return the deepest subtree's root, and both return null only
    // for a null root - which no example above has, since a level-order array names
    // its root in slot 0. IsType asks for that node and fails the test if it is
    // absent, rather than promising it to the compiler.
    private static BinaryTreeNode<int> SubtreeRoot(BinaryTreeNode<int>? subtree) =>
        Assert.IsType<BinaryTreeNode<int>>(subtree);

    // LeetCode's level-order array shape: each existing node consumes exactly two
    // subsequent slots for its children, null marking a missing one.
    private static BinaryTreeNode<int>? BuildTree(int?[] levelOrder)
    {
        if (levelOrder.Length == 0 || levelOrder[0] is null)
        {
            return null;
        }

        var root = new BinaryTreeNode<int>(levelOrder[0].Value);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);
        var i = 1;

        while (queue.Count > 0 && i < levelOrder.Length)
        {
            var node = queue.Dequeue();
            i = AttachChildren(node, levelOrder, queue, i);
        }

        return root;
    }

    private static int AttachChildren(
        BinaryTreeNode<int> node, int?[] levelOrder, Queue<BinaryTreeNode<int>> queue, int i)
    {
        if (i < levelOrder.Length && levelOrder[i] is int leftValue)
        {
            node.Left = new BinaryTreeNode<int>(leftValue);
            queue.Enqueue(node.Left);
        }

        i++;

        if (i < levelOrder.Length && levelOrder[i] is int rightValue)
        {
            node.Right = new BinaryTreeNode<int>(rightValue);
            queue.Enqueue(node.Right);
        }

        return i + 1;
    }
}
