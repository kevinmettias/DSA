using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FindModeInBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindModeInBinarySearchTree;

// Harness only. Both mode-finding strategies are
// FindModeInBinarySearchTreeSolution's; this file pins them to LeetCode's
// published examples, given in LeetCode's own level-order-with-null array shape.
// BinaryTreeNode<int> is internal, so - as in BinaryTreeLevelOrderTraversalTests -
// it stays out of a public TheoryData signature and BuildTree reconstructs each
// example fresh per theory row (CS0053 is why this file used one [Fact] per example
// before). LeetCode accepts the modes in any order, so both strategies' results are
// sorted before comparing - the hash-map strategy in particular has no reason to
// come out in ascending order the way the in-order-walk strategy naturally does.
public sealed class FindModeInBinarySearchTreeTests
{
    public static TheoryData<TreeExample> Examples =>
        new()
        {
            // [1,null,2,2] -> [2]
            { new TreeExample([1, null, 2, 2], [2]) },

            // [2,1,3,1,null,null,3] -> [1,3], one tie in each subtree
            // (LeetCode writes it [2,1,3,1,null,3], eliding the trailing nulls).
            { new TreeExample([2, 1, 3, 1, null, null, 3], [1, 3]) },

            { new TreeExample([7], [7]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindModeByHashMapFrequencyCount_LeetCodeExamples_ReturnsEveryMostFrequentValue(
        TreeExample example) =>
        Assert.Equal(
            example.Expected,
            FindModeInBinarySearchTreeSolution
                .FindModeByHashMapFrequencyCount(BuildTree(example.Values))
                .OrderBy(x => x));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindModeByInOrderTraversalStreak_LeetCodeExamples_ReturnsEveryMostFrequentValue(
        TreeExample example) =>
        Assert.Equal(
            example.Expected,
            FindModeInBinarySearchTreeSolution
                .FindModeByInOrderTraversalStreak(BuildTree(example.Values))
                .OrderBy(x => x));

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

    public readonly record struct TreeExample(int?[] Values, int[] Expected);
}
