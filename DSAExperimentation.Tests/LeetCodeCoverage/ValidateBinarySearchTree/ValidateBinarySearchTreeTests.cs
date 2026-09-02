using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ValidateBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateBinarySearchTree;

// Harness only. The bounds-recursion validation itself is
// ValidateBinarySearchTreeSolution's - this file just pins it to LeetCode's
// published examples, plus the empty-tree edge case the original test never
// exercised. BinaryTreeNode<int> is internal, so - as in
// UniqueBinarySearchTreesIITests - it stays out of a public TheoryData/[Theory]
// signature and is only ever handed to the solution through private helpers.
public sealed class ValidateBinarySearchTreeTests
{
    [Fact]
    public void IsValidByBoundsRecursion_ValidTree_ReturnsTrue() =>
        Assert.True(ValidateBinarySearchTreeSolution.IsValidByBoundsRecursion(ValidTree()));

    [Fact]
    public void IsValidByBoundsRecursion_InvalidTree_ReturnsFalse() =>
        Assert.False(ValidateBinarySearchTreeSolution.IsValidByBoundsRecursion(InvalidTree()));

    [Fact]
    public void IsValidByBoundsRecursion_EmptyTree_ReturnsTrue() =>
        Assert.True(ValidateBinarySearchTreeSolution.IsValidByBoundsRecursion(null));

    private static BinaryTreeNode<int> ValidTree() => new(2) { Left = new(1), Right = new(3) };

    private static BinaryTreeNode<int> InvalidTree() =>
        new(5) { Left = new(1), Right = new(4) { Left = new(3), Right = new(6) } };
}
