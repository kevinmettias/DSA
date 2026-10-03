using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SameTree;

namespace DSAExperimentation.LeetCode.Tests.SameTree;

// Harness only. Two strategies live in SameTreeSolution - the recursive compare and
// the iterative stack compare - so this file pins each to the same five shapes.
// BinaryTreeNode<int> is internal, so it stays out of a public TheoryData/[Theory]
// signature and is only ever handed to the solution through private helpers.
public sealed partial class SameTreeSolutionTests
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

    [Fact]
    public void IsSameByIterativeStackCompare_IdenticalTrees_ReturnsTrue() =>
        Assert.True(SameTreeSolution.IsSameByIterativeStackCompare(Tree(1, Tree(2), Tree(3)), Tree(1, Tree(2), Tree(3))));

    [Fact]
    public void IsSameByIterativeStackCompare_DifferentShape_ReturnsFalse() =>
        Assert.False(SameTreeSolution.IsSameByIterativeStackCompare(Tree(1, Tree(2)), Tree(1, null, Tree(2))));

    [Fact]
    public void IsSameByIterativeStackCompare_SameShapeDifferentValues_ReturnsFalse() =>
        Assert.False(SameTreeSolution.IsSameByIterativeStackCompare(Tree(1, Tree(2), Tree(1)), Tree(1, Tree(1), Tree(2))));

    [Fact]
    public void IsSameByIterativeStackCompare_BothEmpty_ReturnsTrue() =>
        Assert.True(SameTreeSolution.IsSameByIterativeStackCompare(null, null));

    [Fact]
    public void IsSameByIterativeStackCompare_OneEmpty_ReturnsFalse() =>
        Assert.False(SameTreeSolution.IsSameByIterativeStackCompare(Tree(1), null));

    private static BinaryTreeNode<int> Tree(int value, BinaryTreeNode<int>? left = null, BinaryTreeNode<int>? right = null) =>
        new(value) { Left = left, Right = right };
}
