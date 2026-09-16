using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.PathSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSum;

// Harness only. Both strategies live in PathSumSolution - the recursive walk
// (pre-migration, an untested benchmark baseline) and the AllRootToLeafPaths
// composition (pre-migration, the test's own private helper) - and are asserted
// against the same LeetCode examples, so a disagreement between them fails here
// rather than surfacing only as a benchmark/test mismatch. BinaryTreeNode<int> is
// internal, so - as in BinaryTreeLevelOrderTraversalTests - it stays out of a
// public TheoryData signature and LeetCodeWireFormat.ToBinaryTree reconstructs it from LeetCode's own
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
        var hasPath = PathSumSolution.HasPathSumByRecursion(LeetCodeWireFormat.ToBinaryTree(example.Values), example.TargetSum);

        Assert.Equal(example.Expected, hasPath);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPathSumByPathEnumeration_LeetCodeExamples_ReturnsWhetherSomeRootToLeafPathReachesTheTarget(
        TreeExample example)
    {
        var hasPath = PathSumSolution.HasPathSumByPathEnumeration(LeetCodeWireFormat.ToBinaryTree(example.Values), example.TargetSum);

        Assert.Equal(example.Expected, hasPath);
    }

    public readonly record struct TreeExample(int?[] Values, int TargetSum, bool Expected);
}
