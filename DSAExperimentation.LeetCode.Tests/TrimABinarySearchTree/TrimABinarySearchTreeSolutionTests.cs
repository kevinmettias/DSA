using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.TrimABinarySearchTree;

namespace DSAExperimentation.LeetCode.Tests.TrimABinarySearchTree;

// Harness only. Both strategies are TrimABinarySearchTreeSolution's, asserted against
// LeetCode's published examples in its own level-order notation. Comparing the whole
// tree rather than its in-order values is the point: LC 669 requires the kept nodes to
// keep their relative structure, and every BST holding the same values has the same
// in-order sequence - a check on values alone passed a strategy that rebuilt the
// survivors into a chain.
public sealed partial class TrimABinarySearchTreeSolutionTests
{
    public static TheoryData<TrimExample> Examples =>
        new()
        {
            { new TrimExample([1, 0, 2], Low: 1, High: 2, Expected: [1, null, 2]) },
            { new TrimExample([3, 0, 4, null, 2, null, null, 1], Low: 1, High: 3, Expected: [3, 2, null, 1]) },

            // Everything trimmed.
            { new TrimExample([1, 0, 2], Low: 3, High: 5, Expected: []) },

            // Both boundaries cut below the root: 2 goes from under 3, and 8 is replaced
            // by its own left child 7, which keeps its place under 5.
            { new TrimExample([5, 3, 8, 2, 4, 7, 9], Low: 3, High: 7, Expected: [5, 3, 7, null, 4]) },

            // The root itself is out of range, so the answer is rooted further down.
            { new TrimExample([5, 3, 8, 2, 4, 7, 9], Low: 6, High: 9, Expected: [8, 7, 9]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrimByInPlaceMutation_LeetCodeExamples_KeepsTheSurvivorsStructure(TrimExample example)
    {
        var trimmed = TrimABinarySearchTreeSolution.TrimByInPlaceMutation(
            LeetCodeWireFormat.ToBinaryTree(example.LevelOrder), example.Low, example.High);

        Assert.Equal(example.Expected, LeetCodeWireFormat.FromBinaryTree(trimmed));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrimByIterativeBoundaryWalk_LeetCodeExamples_KeepsTheSurvivorsStructure(TrimExample example)
    {
        var trimmed = TrimABinarySearchTreeSolution.TrimByIterativeBoundaryWalk(
            LeetCodeWireFormat.ToBinaryTree(example.LevelOrder), example.Low, example.High);

        Assert.Equal(example.Expected, LeetCodeWireFormat.FromBinaryTree(trimmed));
    }

    // One LeetCode example: the tree in LeetCode's level order, the range to keep, and the
    // trimmed tree in the same notation. Low and High are both ints, so each is named.
    public readonly record struct TrimExample(int?[] LevelOrder, int Low, int High, int?[] Expected);
}
