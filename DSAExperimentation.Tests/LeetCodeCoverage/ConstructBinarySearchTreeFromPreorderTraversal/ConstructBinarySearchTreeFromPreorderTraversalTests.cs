using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructBinarySearchTreeFromPreorderTraversal;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinarySearchTreeFromPreorderTraversal;

// Harness only. Both construction strategies are
// ConstructBinarySearchTreeFromPreorderTraversalSolution's; this file pins them to
// LeetCode's published examples. The built tree is checked twice - its preorder
// must reproduce the input (that is what "this is the tree that input came from"
// means) and its in-order must come out ascending (that is what makes it a BST) -
// because preorder alone does not prove the ordering invariant. Reading a built
// tree back out is result inspection, not the algorithm, which stays in tier 4.
public sealed class ConstructBinarySearchTreeFromPreorderTraversalTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [8, 5, 1, 7, 10, 12], [8, 5, 1, 7, 10, 12], [1, 5, 7, 8, 10, 12] },
            { [1, 3], [1, 3], [1, 3] },
            { [1], [1], [1] },

            // Strictly descending: a fully left-skewed tree.
            { [5, 4, 3, 2, 1], [5, 4, 3, 2, 1], [1, 2, 3, 4, 5] },

            // Strictly ascending: a fully right-skewed tree, the shape the
            // benchmark measures.
            { [1, 2, 3, 4, 5], [1, 2, 3, 4, 5], [1, 2, 3, 4, 5] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BstFromPreorderByRepeatedInsert_LeetCodeExamples_RebuildsTheSourceTree(
        int[] preorder, int[] expectedPreorder, int[] expectedInOrder)
    {
        var root = ConstructBinarySearchTreeFromPreorderTraversalSolution
            .BstFromPreorderByRepeatedInsert(preorder);

        Assert.Equal(expectedPreorder, PreOrder(root));
        Assert.Equal(expectedInOrder, InOrder(root));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void BstFromPreorderByUpperBoundRecursion_LeetCodeExamples_RebuildsTheSourceTree(
        int[] preorder, int[] expectedPreorder, int[] expectedInOrder)
    {
        var root = ConstructBinarySearchTreeFromPreorderTraversalSolution
            .BstFromPreorderByUpperBoundRecursion(preorder);

        Assert.Equal(expectedPreorder, PreOrder(root));
        Assert.Equal(expectedInOrder, InOrder(root));
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root) =>
        root is null ? [] : [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];

    private static int[] InOrder(BinaryTreeNode<int>? root) =>
        root is null ? [] : [.. InOrder(root.Left), root.Value, .. InOrder(root.Right)];
}
