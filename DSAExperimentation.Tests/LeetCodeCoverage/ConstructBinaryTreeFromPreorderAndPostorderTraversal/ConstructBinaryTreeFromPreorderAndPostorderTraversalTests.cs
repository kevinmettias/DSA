using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructBinaryTreeFromPreorderAndPostorderTraversal;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinaryTreeFromPreorderAndPostorderTraversal;

// Harness only: both strategies are
// ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution's. LC 889 accepts
// any tree matching both traversals, so each example is checked by re-flattening
// the reconstructed tree and requiring it to round-trip to the two inputs - which
// also pins the leaf cases, since any spurious child would show up in the walks.
public sealed class ConstructBinaryTreeFromPreorderAndPostorderTraversalTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 4, 5, 3, 6, 7], [4, 5, 2, 6, 7, 3, 1] },
            { [1], [1] },
            { [1, 2], [2, 1] },
            { [4, 3, 2, 1], [1, 2, 3, 4] },
            { [1, 2, 3, 4, 5], [4, 5, 3, 2, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildByPostorderScan_LeetCodeExamples_ReconstructsBinaryTree(int[] preorder, int[] postorder)
    {
        var root = ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution.BuildByPostorderScan(
            preorder, postorder);

        Assert.Equal(preorder, PreOrder(root));
        Assert.Equal(postorder, PostOrder(root));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildByPostorderIndexMap_LeetCodeExamples_ReconstructsBinaryTree(int[] preorder, int[] postorder)
    {
        var root = ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution.BuildByPostorderIndexMap(
            preorder, postorder);

        Assert.Equal(preorder, PreOrder(root));
        Assert.Equal(postorder, PostOrder(root));
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return [];
        }

        return [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
    }

    private static int[] PostOrder(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return [];
        }

        return [.. PostOrder(root.Left), .. PostOrder(root.Right), root.Value];
    }
}
