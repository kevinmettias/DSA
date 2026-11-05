using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.FindModeInBinarySearchTree;

// LC 501's promise about its input: the tree is a binary search tree, where a left subtree holds
// values no larger than its node and a right subtree values no smaller. LeetCode's judge never poses
// an input that breaks it. The in-order streak counts a value's run, which a tree out of order
// splits. Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class FindModeInBinarySearchTreePromise
{
    public static bool IsKeptBy(BinaryTreeNode<int>? root) => !InOrderTraversal.Walk(root, new OrderHooks()).IsOutOfOrder;

    // Walked in order, notes any value smaller than the one before it. Mutable by design: the walk
    // hands back the value it finished with.
    private struct OrderHooks : IInOrderHooks<int>
    {
        private int? _previous;

        public bool IsOutOfOrder { get; private set; }

        public void Visit(BinaryTreeNode<int> node, int depth)
        {
            IsOutOfOrder |= _previous > node.Value;
            _previous = node.Value;
        }
    }
}
