using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSum;

// Harness only. Both strategies live in PathSumSolution - the recursive walk
// (pre-migration, an untested benchmark baseline) and the AllRootToLeafPaths
// composition (pre-migration, the test's own private helper) - and are asserted
// against the same LeetCode examples, so a disagreement between them fails here
// rather than surfacing only as a benchmark/test mismatch. BinaryTreeNode<int> is
// internal, so - as in BinaryTreeLevelOrderTraversalTests - it stays out of a
// public TheoryData signature and BuildTree reconstructs it from LeetCode's own
// level-order-with-null array shape.
public sealed class PathSumTests
{
    // LeetCode 112's own examples: root = [5,4,8,11,null,13,4,7,2,null,null,null,1]
    // with targetSum 22 -> true (5 -> 4 -> 11 -> 2), root = [1,2,3] with targetSum 5
    // -> false (neither root-to-leaf path, 1->2 = 3 and 1->3 = 4, reaches it), and
    // the empty tree with targetSum 0 -> false.
    public static TheoryData<TreeExample> Examples =>
        new()
        {
            { new TreeExample([5, 4, 8, 11, null, 13, 4, 7, 2, null, null, null, 1], 22, true) },
            { new TreeExample([1, 2, 3], 5, false) },
            { new TreeExample([], 0, false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPathSumByRecursion_LeetCodeExamples_ReturnsWhetherSomeRootToLeafPathReachesTheTarget(
        TreeExample example)
    {
        var hasPath = PathSumSolution.HasPathSumByRecursion(BuildTree(example.Values), example.TargetSum);

        Assert.Equal(example.Expected, hasPath);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPathSumByPathEnumeration_LeetCodeExamples_ReturnsWhetherSomeRootToLeafPathReachesTheTarget(
        TreeExample example)
    {
        var hasPath = PathSumSolution.HasPathSumByPathEnumeration(BuildTree(example.Values), example.TargetSum);

        Assert.Equal(example.Expected, hasPath);
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

    public readonly record struct TreeExample(int?[] Values, int TargetSum, bool Expected);
}
