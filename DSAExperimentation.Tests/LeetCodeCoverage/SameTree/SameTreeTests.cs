using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SameTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SameTree;

// Harness only. The pre-migration benchmark carried two [Benchmark] methods that
// both called the same private recursive comparison - one strategy, not two - so
// SameTreeSolution has a single method. BinaryTreeNode<int> is internal, so - as
// in ValidateBinarySearchTreeTests - it stays out of a public TheoryData/[Theory]
// signature and is only ever handed to the solution through private helpers.
public sealed partial class SameTreeTests
{
    [Fact]
    public void IsSameByRecursiveCompare_IdenticalTrees_ReturnsTrue()
    {
        var first = Tree(1, Tree(2), Tree(3));
        var second = Tree(1, Tree(2), Tree(3));
        var same = SameTreeSolution.IsSameByRecursiveCompare(first, second);

        Assert.True(same);
    }

    [Fact]
    public void IsSameByRecursiveCompare_DifferentShape_ReturnsFalse()
    {
        var leftLeaning = Tree(1, Tree(2), null);
        var rightLeaning = Tree(1, null, Tree(2));
        var same = SameTreeSolution.IsSameByRecursiveCompare(leftLeaning, rightLeaning);

        Assert.False(same);
    }

    [Fact]
    public void IsSameByRecursiveCompare_SameShapeDifferentValues_ReturnsFalse()
    {
        var first = Tree(1, Tree(2), Tree(1));
        var second = Tree(1, Tree(1), Tree(2));
        var same = SameTreeSolution.IsSameByRecursiveCompare(first, second);

        Assert.False(same);
    }

    [Fact]
    public void IsSameByRecursiveCompare_BothEmpty_ReturnsTrue()
    {
        var same = SameTreeSolution.IsSameByRecursiveCompare(null, null);

        Assert.True(same);
    }

    [Fact]
    public void IsSameByRecursiveCompare_OneEmpty_ReturnsFalse()
    {
        var same = SameTreeSolution.IsSameByRecursiveCompare(Tree(1), null);

        Assert.False(same);
    }

    private static BinaryTreeNode<int> Tree(int value, BinaryTreeNode<int>? left = null, BinaryTreeNode<int>? right = null) =>
        new(value) { Left = left, Right = right };
}
