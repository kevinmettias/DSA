using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MinimumAbsoluteDifferenceInBST;

// LeetCode 530. Minimum Absolute Difference in BST: an in-order walk of a BST
// visits values in ascending order, so the minimum absolute difference between any
// two nodes is always between some adjacent pair in that walk.
internal static class MinimumAbsoluteDifferenceInBSTSolution
{

    // The textbook answer: collect every value with a hand-rolled recursive
    // in-order walk, then scan the resulting list for the smallest adjacent gap.
    // Deliberately written without this repo's traversal primitives - it is the
    // arm the composed solution below has to justify itself against.
    public static int GetMinimumDifferenceByRecursiveScan(BinaryTreeNode<int> root)
    {
        var values = new List<int>();
        CollectInOrder(root, values);

        var minDiff = int.MaxValue;
        for (var i = 1; i < values.Count; i++)
        {
            minDiff = Math.Min(minDiff, values[i] - values[i - 1]);
        }

        return minDiff;
    }

    // This repo's own InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>,
    // tracking the previous value across Visit calls instead of materializing the
    // whole in-order sequence first - the same composition
    // RecoverBinarySearchTreeSolution uses.
    public static int GetMinimumDifferenceByInOrderHooks(BinaryTreeNode<int> root)
    {
        State.Prev.Value = null;
        State.MinDiff.Value = int.MaxValue;

        InOrderTraversal.Walk<int, DiffHooks>(root);

        return State.MinDiff.Value;
    }

    private static void CollectInOrder(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        CollectInOrder(node.Left, values);
        values.Add(node.Value);
        CollectInOrder(node.Right, values);
    }

    private readonly struct DiffHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Prev.Value is { } prev)
            {
                State.MinDiff.Value = Math.Min(State.MinDiff.Value, node.Value - prev.Value);
            }

            State.Prev.Value = node;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Prev = new();
        public static readonly AsyncLocal<int> MinDiff = new();
    }
}
