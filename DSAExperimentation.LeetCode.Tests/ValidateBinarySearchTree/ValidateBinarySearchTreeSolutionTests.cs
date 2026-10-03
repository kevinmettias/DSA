using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ValidateBinarySearchTree;

namespace DSAExperimentation.LeetCode.Tests.ValidateBinarySearchTree;

// Harness only. The bounds-recursion validation itself is
// ValidateBinarySearchTreeSolution's - this file just pins it to LeetCode's
// published examples, given in LeetCode's own level-order-with-null array shape,
// plus the empty-tree edge case the original test never exercised.
// BinaryTreeNode<int> is internal, so - as in UniqueBinarySearchTreesIITests - it
// stays out of a public TheoryData signature and LeetCodeWireFormat.ToBinaryTree reconstructs it from
// that array.
public sealed partial class ValidateBinarySearchTreeSolutionTests
{
    public static TheoryData<TreeExample> Examples =>
        new()
        {
            { new TreeExample(Values: [2, 1, 3], Expected: true) },
            { new TreeExample(Values: [5, 1, 4, null, null, 3, 6], Expected: false) },
            { new TreeExample(Values: [], Expected: true) },
            { new TreeExample(Values: [1], Expected: true) },

            // The case a naive "left child < parent < right child" check passes and a
            // real bounds check does not: 3 is below its own parent 4, but it sits in
            // 5's right subtree.
            { new TreeExample(Values: [5, 4, 6, null, null, 3, 7], Expected: false) },
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
