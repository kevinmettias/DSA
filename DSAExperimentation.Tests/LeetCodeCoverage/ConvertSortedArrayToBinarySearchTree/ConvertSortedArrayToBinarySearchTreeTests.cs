using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertSortedArrayToBinarySearchTree;

public sealed partial class ConvertSortedArrayToBinarySearchTreeTests
{
    [Fact]
    public void SortedArrayToBST_ClassicExample_ProducesHeightBalancedBst()
    {
        var root = Build([-10, -3, 0, 5, 9]);
        Assert.Equal([-10, -3, 0, 5, 9], InOrder(root));
        Assert.True(Math.Abs(Height(root!.Left) - Height(root.Right)) <= 1);
    }

    private static BinaryTreeNode<int>? Build(int[] nums) => Build(nums, 0, nums.Length - 1);
    private static BinaryTreeNode<int>? Build(int[] nums, int low, int high) { if (low > high) return null; var mid = low + ((high - low) / 2); return new BinaryTreeNode<int>(nums[mid]) { Left = Build(nums, low, mid - 1), Right = Build(nums, mid + 1, high) }; }
    private static int Height(BinaryTreeNode<int>? node) => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));
    private static int[] InOrder(BinaryTreeNode<int>? node) => node is null ? [] : [.. InOrder(node.Left), node.Value, .. InOrder(node.Right)];
}
