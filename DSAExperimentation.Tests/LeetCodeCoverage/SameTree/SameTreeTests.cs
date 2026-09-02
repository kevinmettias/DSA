using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SameTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SameTree;

// Harness only. The pre-migration benchmark carried two [Benchmark] methods that
// both called the same private recursive comparison - one strategy, not two - so
// SameTreeSolution has a single method. BinaryTreeNode<int> is internal, so - as
// in ValidateBinarySearchTreeTests - it stays out of a public TheoryData/[Theory]
// signature and is only ever handed to the solution through private helpers.
public sealed class SameTreeTests
{
    [Fact]
    public void IsSameByRecursiveCompare_IdenticalTrees_ReturnsTrue() =>
        Assert.True(SameTreeSolution.IsSameByRecursiveCompare(
            Tree(1, Tree(2), Tree(3)), Tree(1, Tree(2), Tree(3))));

    [Fact]
    public void IsSameByRecursiveCompare_DifferentShape_ReturnsFalse() =>
        Assert.False(SameTreeSolution.IsSameByRecursiveCompare(
            Tree(1, Tree(2), null), Tree(1, null, Tree(2))));

    [Fact]
    public void IsSameByRecursiveCompare_SameShapeDifferentValues_ReturnsFalse() =>
        Assert.False(SameTreeSolution.IsSameByRecursiveCompare(
            Tree(1, Tree(2), Tree(1)), Tree(1, Tree(1), Tree(2))));

    [Fact]
    public void IsSameByRecursiveCompare_BothEmpty_ReturnsTrue() =>
        Assert.True(SameTreeSolution.IsSameByRecursiveCompare(null, null));

    [Fact]
    public void IsSameByRecursiveCompare_OneEmpty_ReturnsFalse() =>
        Assert.False(SameTreeSolution.IsSameByRecursiveCompare(Tree(1), null));

    private static BinaryTreeNode<int> Tree(int value, BinaryTreeNode<int>? left = null, BinaryTreeNode<int>? right = null) =>
        new(value) { Left = left, Right = right };
}
