using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.AllElementsInTwoBinarySearchTrees;

// LeetCode 1305. All Elements in Two Binary Search Trees: every value from both
// trees, in ascending order, keeping duplicates.
//
// An in-order walk of a BST already visits values in ascending order, so the only
// real question is whether that ordering is exploited or thrown away and
// recovered with a sort.
internal static class AllElementsInTwoBinarySearchTreesSolution
{
    // The textbook baseline: dump every node from both trees into one list and sort
    // it - O((n+m) log(n+m)), and it ignores the BST ordering entirely. Its
    // internals are deliberately BCL (a recursive pre-order walk, List<int>,
    // Array.Sort); only the trees it is handed are this repo's types.
    public static int[] GetAllElementsByCollectThenSort(BinaryTreeNode<int>? root1, BinaryTreeNode<int>? root2)
    {
        var values = new List<int>();
        CollectAll(root1, values);
        CollectAll(root2, values);

        var result = values.ToArray();
        Array.Sort(result);
        return result;
    }

    private static void CollectAll(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        values.Add(node.Value);
        CollectAll(node.Left, values);
        CollectAll(node.Right, values);
    }

    // This repo's own InOrderTraversal/IInOrderHooks composition - the same one
    // KthSmallestElementInABSTSolution and FindModeInBinarySearchTreeSolution use -
    // buffering each tree's already-ascending sequence into a DynamicArray<int>,
    // then a plain two-pointer merge of the two sorted sequences. O(n+m), no sort.
    public static int[] GetAllElementsByInOrderMerge(BinaryTreeNode<int>? root1, BinaryTreeNode<int>? root2)
    {
        var first = CollectInOrder(root1);
        var second = CollectInOrder(root2);
        return MergeSorted(first, second);
    }

    private static DynamicArray<int> CollectInOrder(BinaryTreeNode<int>? root)
    {
        State.Values.Value = new DynamicArray<int>();
        InOrderTraversal.Walk<int, CollectHooks>(root);
        return State.Values.Value;
    }

    private static int[] MergeSorted(DynamicArray<int> first, DynamicArray<int> second)
    {
        var result = new int[first.Count + second.Count];
        var i = 0;
        var j = 0;
        var k = 0;

        while (i < first.Count && j < second.Count)
        {
            result[k++] = first.Get(i) <= second.Get(j) ? first.Get(i++) : second.Get(j++);
        }

        while (i < first.Count)
        {
            result[k++] = first.Get(i++);
        }

        while (j < second.Count)
        {
            result[k++] = second.Get(j++);
        }

        return result;
    }

    // Hooks are static, so the buffer being filled lives in AsyncLocal state
    // alongside the walk - the same arrangement KthSmallestElementInABSTSolution
    // uses for its running rank.
    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Values.Value!.Add(node.Value);
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Values = new();
    }
}
