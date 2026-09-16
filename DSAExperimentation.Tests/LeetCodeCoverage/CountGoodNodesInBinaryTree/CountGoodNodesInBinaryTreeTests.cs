using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.CountGoodNodesInBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountGoodNodesInBinaryTree;

// LeetCode 1448. Count Good Nodes in Binary Tree. See
// CountGoodNodesInBinaryTreeSolution for the two strategies: a plain recursive DFS
// threading the running maximum through call-stack parameters, and this repo's own
// TopDownTraversal threading it through ITopDownHooks.Descend.
//
// Examples are stated as LeetCode's own level-order arrays - BinaryTreeNode<int> is
// internal, so it cannot appear in a public TheoryData member; BuildTree
// reconstructs the tree inside each test method instead, the same shape
// FindElementsInAContaminatedBinaryTreeTests (LC 1261) uses.
public sealed class CountGoodNodesInBinaryTreeTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            // LeetCode example 1: [3,1,4,3,null,1,5] - the root, the 4, the 3 under
            // the 1, and the 5 are good; the 1s are not.
            { [3, 1, 4, 3, null, 1, 5], 4 },

            // The same tree with the 3 hanging off the 1's other side - the shape the
            // pre-migration test asserted, pinning that goodness depends on ancestry
            // rather than on which side a child sits.
            { [3, 1, 4, null, 3, 1, 5], 4 },

            // LeetCode example 2: [3,3,null,4,2] - only the 2 is outranked.
            { [3, 3, null, 4, 2], 3 },

            // LeetCode example 3: a lone root is always good.
            { [1], 1 },

            // A strictly decreasing chain: every descendant is outranked, so only the
            // root counts.
            { [3, 1, null, 0], 1 },

            // The same shape with a single node carrying a different value, pinning
            // that the seed is "no ancestor" rather than any particular number.
            { [5], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodNodesByRecursiveDfs_LeetCodeExamples_CountsNodesNoAncestorOutranks(
        int?[] levelOrder, int expected) =>
        Assert.Equal(
            expected,
            CountGoodNodesInBinaryTreeSolution.CountGoodNodesByRecursiveDfs(BuildTree(levelOrder)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodNodesByTopDownTraversal_LeetCodeExamples_CountsNodesNoAncestorOutranks(
        int?[] levelOrder, int expected) =>
        Assert.Equal(
            expected,
            CountGoodNodesInBinaryTreeSolution.CountGoodNodesByTopDownTraversal(BuildTree(levelOrder)));

    // LeetCode's own level-order input shape: a BFS-ordered array with null standing
    // in for a missing child. The shape names the root in slot 0, so an array that
    // opens with null describes no tree for this helper to build.
    private static BinaryTreeNode<int> BuildTree(int?[] levelOrder)
    {
        var rootValue = levelOrder[0]
            ?? throw new InvalidOperationException(
                "every example above opens with its root, and the level-order shape names the root in slot 0");

        var root = new BinaryTreeNode<int>(rootValue);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);

        var i = 1;
        while (i < levelOrder.Length)
        {
            i = AttachChildren(levelOrder, i, queue);
        }

        return root;
    }

    // Consumes one slot for each of the dequeued parent's children and returns the
    // index just past them: a null or absent slot attaches nothing but is still spent.
    private static int AttachChildren(int?[] levelOrder, int i, Queue<BinaryTreeNode<int>> queue)
    {
        var parent = queue.Dequeue();

        if (i < levelOrder.Length && levelOrder[i] is { } leftValue)
        {
            parent.Left = new BinaryTreeNode<int>(leftValue);
            queue.Enqueue(parent.Left);
        }

        i++;

        if (i < levelOrder.Length && levelOrder[i] is { } rightValue)
        {
            parent.Right = new BinaryTreeNode<int>(rightValue);
            queue.Enqueue(parent.Right);
        }

        return i + 1;
    }
}
