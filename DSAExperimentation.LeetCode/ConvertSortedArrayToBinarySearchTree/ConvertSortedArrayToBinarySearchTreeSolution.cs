using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.ConvertSortedArrayToBinarySearchTree;

// LeetCode 108. Convert Sorted Array to Binary Search Tree: build a
// height-balanced BST from an ascending array, height-balanced meaning every
// node's two subtree heights differ by at most one.
//
// Picking the midpoint of each remaining range as the subtree root guarantees
// that balance for free - the left and right ranges can differ in length by at
// most one element, every time the range is split. There is only one strategy
// here: the original test's private helper and both of the original
// benchmark's [Benchmark] arms all constructed the tree exactly this way.
internal static class ConvertSortedArrayToBinarySearchTreeSolution
{

    public static BinaryTreeNode<int>? BuildByMidpointRecursion(int[] nums) =>
        BuildByMidpointRecursion(nums, 0, nums.Length - 1);

    private static BinaryTreeNode<int>? BuildByMidpointRecursion(int[] nums, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

        return new BinaryTreeNode<int>(nums[mid])
        {
            Left = BuildByMidpointRecursion(nums, low, mid - 1),
            Right = BuildByMidpointRecursion(nums, mid + 1, high),
        };
    }
}
