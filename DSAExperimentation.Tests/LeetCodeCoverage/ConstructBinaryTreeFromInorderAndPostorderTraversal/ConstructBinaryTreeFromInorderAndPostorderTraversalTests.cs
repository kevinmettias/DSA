using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinaryTreeFromInorderAndPostorderTraversal;

public sealed partial class ConstructBinaryTreeFromInorderAndPostorderTraversalTests
{
    [Fact]
    public void BuildTree_ClassicExample_ReconstructsBinaryTree()
    {
        var root = Build([9, 3, 15, 20, 7], [9, 15, 7, 20, 3]);
        Assert.Equal([3, 9, 20, 15, 7], PreOrder(root));
    }

    private static BinaryTreeNode<int>? Build(int[] inorder, int[] postorder)
    {
        var positions = new HashMap<int, int>();
        for (var i = 0; i < inorder.Length; i++) positions.Set(inorder[i], i);
        var post = postorder.Length - 1;
        BinaryTreeNode<int>? BuildRange(int low, int high)
        {
            if (low > high) return null;
            var value = postorder[post--]; positions.TryGetValue(value, out var mid);
            return new BinaryTreeNode<int>(value) { Right = BuildRange(mid + 1, high), Left = BuildRange(low, mid - 1) };
        }
        return BuildRange(0, inorder.Length - 1);
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root) => root is null ? [] : [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
}
