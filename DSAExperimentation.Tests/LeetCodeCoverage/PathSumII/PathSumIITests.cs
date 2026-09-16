using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSumII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSumII;

// Harness only. Both strategies are PathSumIISolution's; this file pins them to
// LeetCode's published examples, given in LeetCode's own level-order-with-null
// array shape. BinaryTreeNode<int> is internal, so - as in
// ValidateBinarySearchTreeTests - it stays out of a public TheoryData signature
// and BuildTree reconstructs it from that array. Path order isn't part of
// LeetCode's contract ("return the paths in any order"), so assertions check
// membership and count rather than a fixed sequence.
public sealed class PathSumIITests
{
    public static TheoryData<PathSumExample> Examples =>
        new()
        {
            // [5,4,8,11,null,13,4,7,2,null,null,5,1] - LeetCode's own example 1.
            {
                new PathSumExample(
                    Values: [5, 4, 8, 11, null, 13, 4, 7, 2, null, null, 5, 1],
                    TargetSum: 22,
                    Expected: [[5, 4, 11, 2], [5, 8, 4, 5]])
            },
            // [1,2,3] - LeetCode's own example 2 (targetSum 5 matches neither leaf path).
            { new PathSumExample(Values: [1, 2, 3], TargetSum: 5, Expected: []) },
            // [1,2] - LeetCode's own example 3 (the only leaf path, 1+2 = 3, is not 0).
            { new PathSumExample(Values: [1, 2], TargetSum: 0, Expected: []) },
            // A single node that is itself the target sum: the path is just the root.
            { new PathSumExample(Values: [5], TargetSum: 5, Expected: [[5]]) },
            { new PathSumExample(Values: [], TargetSum: 0, Expected: []) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPathsByRecursiveBacktrack_LeetCodeExamples_ReturnsEveryMatchingRootToLeafPath(
        PathSumExample example)
    {
        var paths = PathSumIISolution.FindPathsByRecursiveBacktrack(BuildTree(example.Values), example.TargetSum);

        AssertMatches(example.Expected, paths);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPathsByAllRootToLeafPaths_LeetCodeExamples_ReturnsEveryMatchingRootToLeafPath(
        PathSumExample example)
    {
        var paths = PathSumIISolution.FindPathsByAllRootToLeafPaths(BuildTree(example.Values), example.TargetSum);

        AssertMatches(example.Expected, paths);
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

    private static void AssertMatches(int[][] expected, List<List<int>> actual)
    {
        Assert.Equal(expected.Length, actual.Count);

        foreach (var path in expected)
        {
            Assert.Contains(actual, p => p.SequenceEqual(path));
        }
    }

    // One LeetCode example: the tree in LeetCode's level-order-with-null array
    // shape, the target sum, and every root-to-leaf path whose values sum to it.
    public readonly record struct PathSumExample(int?[] Values, int TargetSum, int[][] Expected);
}
