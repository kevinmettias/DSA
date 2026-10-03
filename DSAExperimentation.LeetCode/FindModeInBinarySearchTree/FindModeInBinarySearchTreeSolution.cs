using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.FindModeInBinarySearchTree;

// LeetCode 501. Find Mode in Binary Search Tree: find every value that appears
// most often in a BST that may contain duplicates. LeetCode accepts the modes in
// any order.
//
// The general "any binary tree" fix is a full traversal building a frequency map,
// then a second pass to find the max and collect ties - the arm the BST-aware
// strategy has to justify itself against. The BST-aware strategy exploits the
// fact that an in-order walk of a BST visits equal values consecutively, so
// tracking a running streak length while walking finds every most-frequent value
// in one O(n) pass with no extra map: this repo's own
// InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>, the same composition
// KthSmallestElementInABSTSolution uses, collecting modes into a
// DynamicArray<int> instead of a BCL List<int>.
internal static class FindModeInBinarySearchTreeSolution
{
    // The textbook fix for an arbitrary binary tree: a Dictionary<int,int>
    // frequency count over the whole tree, then a second pass to find the max
    // and collect every value tied for it. Written without this repo's traversal
    // engine and without exploiting the BST ordering at all.
    public static int[] FindModeByHashMapFrequencyCount(BinaryTreeNode<int>? root)
    {
        var counts = new Dictionary<int, int>();
        CountFrequencies(root, counts);

        var maxCount = 0;
        foreach (var count in counts.Values)
        {
            if (count > maxCount)
            {
                maxCount = count;
            }
        }

        var modes = new List<int>();
        foreach (var (value, count) in counts)
        {
            if (count == maxCount)
            {
                modes.Add(value);
            }
        }

        return modes.ToArray();
    }

    // This repo's own InOrderTraversal/IInOrderHooks composition. The running value,
    // streak, longest streak and modes live in the hook, which the walk hands back.
    public static int[] FindModeByInOrderTraversalStreak(BinaryTreeNode<int>? root)
    {
        var modes = InOrderTraversal.Walk(root, new ModeHooks()).Modes;
        var result = new int[modes.Count];

        for (var i = 0; i < modes.Count; i++)
        {
            result[i] = modes.Get(i);
        }

        return result;
    }

    private static void CountFrequencies(BinaryTreeNode<int>? node, Dictionary<int, int> counts)
    {
        if (node is null)
        {
            return;
        }

        counts[node.Value] = counts.GetValueOrDefault(node.Value) + 1;
        CountFrequencies(node.Left, counts);
        CountFrequencies(node.Right, counts);
    }

    // Tracks the run of equal values the walk is in, the longest run so far, and every value
    // whose run reached that length. Mutable by design: the walk hands back the value it
    // finished with.
    private struct ModeHooks() : IInOrderHooks<int>
    {
        private int _currentValue;
        private int _currentCount;
        private int _maxCount;

        public DynamicArray<int> Modes { get; private set; } = new();

        public void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (_currentCount == 0 || node.Value != _currentValue)
            {
                _currentValue = node.Value;
                _currentCount = 0;
            }

            _currentCount++;

            if (_currentCount > _maxCount)
            {
                _maxCount = _currentCount;
                Modes = new DynamicArray<int>();
                Modes.Add(node.Value);
            }
            else if (_currentCount == _maxCount)
            {
                Modes.Add(node.Value);
            }
        }
    }
}
