using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.ValidateBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateBinarySearchTree;

// Harness only. The bounds-recursion validation itself is
// ValidateBinarySearchTreeSolution's - this file just pins it to LeetCode's
// published examples, given in LeetCode's own level-order-with-null array shape,
// plus the empty-tree edge case the original test never exercised.
// BinaryTreeNode<int> is internal, so - as in UniqueBinarySearchTreesIITests - it
// stays out of a public TheoryData signature and LeetCodeWireFormat.ToBinaryTree reconstructs it from
// that array.
public sealed class ValidateBinarySearchTreeTests
{
    public static TheoryData<TreeExample> Examples =>
        new()
        {
            { new TreeExample(Values: [2, 1, 3], Expected: true) },
            { new TreeExample(Values: [5, 1, 4, null, null, 3, 6], Expected: false) },
            { new TreeExample(Values: [], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByBoundsRecursion_Examples_ReturnsWhetherEveryNodeStaysWithinItsBounds(TreeExample example)
    {
        var isValid = ValidateBinarySearchTreeSolution.IsValidByBoundsRecursion(LeetCodeWireFormat.ToBinaryTree(example.Values));

        Assert.Equal(example.Expected, isValid);
    }

    // One example: the tree in LeetCode's level-order-with-null array shape and
    // whether the bounds recursion should accept it as a binary search tree.
    public readonly record struct TreeExample(int?[] Values, bool Expected);
}
