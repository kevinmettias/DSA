using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.PathSumII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSumII;

// Harness only. Both strategies are PathSumIISolution's; this file pins them to
// LeetCode's published examples, given in LeetCode's own level-order-with-null
// array shape. BinaryTreeNode<int> is internal, so - as in
// ValidateBinarySearchTreeTests - it stays out of a public TheoryData signature
// and LeetCodeWireFormat.ToBinaryTree reconstructs it from that array. Path order isn't part of
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
        var paths = PathSumIISolution.FindPathsByRecursiveBacktrack(LeetCodeWireFormat.ToBinaryTree(example.Values), example.TargetSum);

        AssertMatches(example.Expected, paths);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPathsByAllRootToLeafPaths_LeetCodeExamples_ReturnsEveryMatchingRootToLeafPath(
        PathSumExample example)
    {
        var paths = PathSumIISolution.FindPathsByAllRootToLeafPaths(LeetCodeWireFormat.ToBinaryTree(example.Values), example.TargetSum);

        AssertMatches(example.Expected, paths);
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
