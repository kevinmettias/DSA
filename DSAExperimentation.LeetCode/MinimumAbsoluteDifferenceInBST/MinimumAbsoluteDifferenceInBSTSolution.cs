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
        => InOrderTraversal.Walk(root, new DiffHooks()).MinDiff;

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

    // Remembers the previous node and the smallest gap seen. Mutable by design: the walk hands
    // back the value it finished with.
    private struct DiffHooks() : IInOrderHooks<int>
    {
        private BinaryTreeNode<int>? _previous;

        public int MinDiff { get; private set; } = int.MaxValue;

        public void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (_previous is { } previous)
            {
                MinDiff = Math.Min(MinDiff, node.Value - previous.Value);
            }

            _previous = node;
        }
    }
}
