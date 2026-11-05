using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Solutions.ConstructBinaryTreeFromPreorderAndInorderTraversal;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ConstructBinaryTreeFromPreorderAndInorderTraversal;

// Harness only: BuildByPreorderIndexMap lives in
// ConstructBinaryTreeFromPreorderAndInorderTraversalSolution. This file checks it
// on LeetCode's first example a second way, by re-flattening the reconstructed tree
// back to preorder and checking it round-trips to the input. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class ConstructBinaryTreeFromPreorderAndInorderTraversalSolutionTests
{
    public static TheoryData<int[], int[], int[]> Cases =>
        new()
        {
            { [3, 9, 20, 15, 7], [9, 3, 15, 20, 7], [3, 9, 20, 15, 7] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void BuildByPreorderIndexMap_Cases_ReconstructsBinaryTree(
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
