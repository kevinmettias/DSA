using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructBinaryTreeFromPreorderAndInorderTraversal;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinaryTreeFromPreorderAndInorderTraversal;

// Harness only: BuildByPreorderIndexMap lives in
// ConstructBinaryTreeFromPreorderAndInorderTraversalSolution. This file pins it to
// LeetCode's published examples by re-flattening the reconstructed tree back to
// preorder and checking it round-trips to the input.
public sealed class ConstructBinaryTreeFromPreorderAndInorderTraversalTests
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

    private static int[] PreOrder(BinaryTreeNode<int>? root) =>
        root is null ? [] : [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
}
