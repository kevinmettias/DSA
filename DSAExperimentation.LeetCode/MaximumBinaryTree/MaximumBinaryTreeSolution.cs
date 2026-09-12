using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using NodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode<int>>;

namespace DSAExperimentation.LeetCode.MaximumBinaryTree;

// LeetCode 654. Maximum Binary Tree: given an array of distinct integers, build
// the tree whose root is the maximum, whose left subtree is built recursively
// from everything to its left, and whose right subtree is built recursively from
// everything to its right.
//
// Both strategies take LeetCode's own input shape directly - int[] - so there is
// no separate hoisted-input overload: the array already IS the prepared input,
// sized once in a benchmark's [GlobalSetup].
internal static class MaximumBinaryTreeSolution
{
    // The textbook definition, taken literally: find the max in the current
    // range, split around it, recurse on both halves. O(n^2) worst case on an
    // ascending input, where the max is always the last element and the left
    // half never shrinks by more than one node per call.
    public static BinaryTreeNode<int>? ConstructByRescanForMax(int[] nums) =>
        ConstructByRescanForMax(nums, 0, nums.Length - 1);

    private static BinaryTreeNode<int>? ConstructByRescanForMax(int[] nums, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var maxIndex = low;

        for (var i = low + 1; i <= high; i++)
        {
            if (nums[i] > nums[maxIndex])
            {
                maxIndex = i;
            }
        }

        return new BinaryTreeNode<int>(nums[maxIndex])
        {
            Left = ConstructByRescanForMax(nums, low, maxIndex - 1),
            Right = ConstructByRescanForMax(nums, maxIndex + 1, high),
        };
    }

    // O(n): each new value pops every smaller node off this repo's own LIFO
    // Stack<T>, chaining them under its own Left (each popped node was already
    // the largest of everything to its left, so the chain preserves that
    // ordering), then attaches under the remaining top's Right before pushing
    // itself. Produces the same tree as the rescan definition without ever
    // re-deriving the split index.
    public static BinaryTreeNode<int>? ConstructByMonotonicStack(int[] nums)
    {
        var stack = new NodeStack();

        foreach (var num in nums)
        {
            PushValue(stack, num);
        }

        BinaryTreeNode<int>? root = null;

        while (stack.TryPop(out var remaining))
        {
            root = remaining;
        }

        return root;
    }

    private static void PushValue(NodeStack stack, int num)
    {
        var node = new BinaryTreeNode<int>(num);

        while (stack.TryPeek(out var smaller) && smaller.Value < num)
        {
            stack.TryPop(out _);
            node.Left = smaller;
        }

        if (stack.TryPeek(out var parent))
        {
            parent.Right = node;
        }

        stack.Push(node);
    }
}
