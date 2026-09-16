using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructBinaryTreeFromPreorderAndInorderTraversal;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinaryTreeFromPreorderAndInorderTraversal;

// Harness only: BuildByPreorderIndexMap lives in
// ConstructBinaryTreeFromPreorderAndInorderTraversalSolution. This file pins it to
// LeetCode's published examples by re-flattening the reconstructed tree back to
// preorder and checking it round-trips to the input.
public sealed partial class ConstructBinaryTreeFromPreorderAndInorderTraversalTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [3, 9, 20, 15, 7], [9, 3, 15, 20, 7], [3, 9, 20, 15, 7] },
            { [-1], [-1], [-1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildByPreorderIndexMap_LeetCodeExamples_ReconstructsBinaryTree(
        int[] preorder, int[] inorder, int[] expectedPreorder)
    {
        var root = ConstructBinaryTreeFromPreorderAndInorderTraversalSolution.BuildByPreorderIndexMap(preorder, inorder);

        Assert.Equal(expectedPreorder, PreOrder(root));
    }

    // Both arms are values: a call that names the empty walk, and a call that names the
    // non-empty one. The non-empty arm stays a call rather than a hoisted local because
    // the condition guards it - a local above the expression would run it every time.
    private static int[] PreOrder(BinaryTreeNode<int>? root) =>
        root is null ? Array.Empty<int>() : PreOrderNonNull(root);

    private static int[] PreOrderNonNull(BinaryTreeNode<int> root) =>
        [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
}
