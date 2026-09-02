using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSum;

// Harness only. Both strategies live in PathSumSolution - the recursive walk
// (pre-migration, an untested benchmark baseline) and the AllRootToLeafPaths
// composition (pre-migration, the test's own private helper) - and are asserted
// against the same LeetCode examples, so a disagreement between them fails here
// rather than surfacing only as a benchmark/test mismatch. BinaryTreeNode<int> is
// internal, so - as in SameTreeTests - it stays out of a public TheoryData/[Theory]
// signature and is only ever handed to the solution through private helpers.
public sealed class PathSumTests
{
    [Fact]
    public void HasPathSumByRecursion_ClassicExample_ReturnsTrue() =>
        Assert.True(PathSumSolution.HasPathSumByRecursion(ClassicExample(), 22));

    [Fact]
    public void HasPathSumByRecursion_NoAcceptingPath_ReturnsFalse() =>
        Assert.False(PathSumSolution.HasPathSumByRecursion(ShallowExample(), 5));

    [Fact]
    public void HasPathSumByRecursion_EmptyTree_ReturnsFalse() =>
        Assert.False(PathSumSolution.HasPathSumByRecursion(null, 0));

    [Fact]
    public void HasPathSumByPathEnumeration_ClassicExample_ReturnsTrue() =>
        Assert.True(PathSumSolution.HasPathSumByPathEnumeration(ClassicExample(), 22));

    [Fact]
    public void HasPathSumByPathEnumeration_NoAcceptingPath_ReturnsFalse() =>
        Assert.False(PathSumSolution.HasPathSumByPathEnumeration(ShallowExample(), 5));

    [Fact]
    public void HasPathSumByPathEnumeration_EmptyTree_ReturnsFalse() =>
        Assert.False(PathSumSolution.HasPathSumByPathEnumeration(null, 0));

    // LeetCode 112's own example: root = [5,4,8,11,null,13,4,7,2,null,null,null,1],
    // targetSum = 22 -> true (5 -> 4 -> 11 -> 2).
    private static BinaryTreeNode<int> ClassicExample() => new(5)
    {
        Left = new(4) { Left = new(11) { Left = new(7), Right = new(2) } },
        Right = new(8) { Left = new(13), Right = new(4) { Right = new(1) } },
    };

    // LeetCode 112's own example: root = [1,2,3], targetSum = 5 -> false. Neither
    // root-to-leaf path (1->2 = 3, 1->3 = 4) reaches the target.
    private static BinaryTreeNode<int> ShallowExample() => new(1) { Left = new(2), Right = new(3) };
}
