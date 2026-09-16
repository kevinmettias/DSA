using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.IncreasingOrderSearchTree;

// LeetCode 897. Increasing Order Search Tree: rearrange a BST in place so the
// smallest value becomes the root and every node has no left child and exactly one
// right child - a right-only chain in ascending order.
//
// An in-order walk of a BST visits values in ascending order, so relinking each
// visited node's Left to null and chaining it onto the previously-visited node's
// Right builds the answer in a single pass. Both strategies below do exactly that
// relink; they differ only in what drives the walk.
//
// IncreasingBstByInOrderHooks composes this repo's own
// InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>, threading the running
// tail through AsyncLocal state - the same rewrite-during-walk composition
// ConvertBSTToGreaterTreeSolution and KthSmallestElementInABSTSolution use.
//
// IncreasingBstByRecursiveRelink is the textbook baseline it is measured against: a
// hand-rolled recursive in-order walk threading the tail through recursive
// parameters and return values, using no repo traversal primitive at all - only the
// tree it is handed is a repo type. Not an asymptotic win either way (both are
// O(n)); the comparison is about walk mechanics, the same shape
// KthSmallestElementInABSTBenchmarks already makes for LC 230. Before this
// migration the baseline lived only inside IncreasingOrderSearchTreeBenchmarks, so
// nothing asserted it.
internal static class IncreasingOrderSearchTreeSolution
{
    // A throwaway head node the first relinked node is chained onto, so the walk
    // needs no "is this the first visit" branch. Its own value is never read.
    private const int DummyHeadValue = 0;

    public static BinaryTreeNode<int>? IncreasingBstByRecursiveRelink(BinaryTreeNode<int>? root)
    {
        var dummy = new BinaryTreeNode<int>(DummyHeadValue);
        VisitAndRelink(root, dummy);

        return dummy.Right;

        static BinaryTreeNode<int> VisitAndRelink(BinaryTreeNode<int>? node, BinaryTreeNode<int> tail)
        {
            if (node is null)
            {
                return tail;
            }

            tail = VisitAndRelink(node.Left, tail);

            // Read Right before the relink: tail.Right = node overwrites it when
            // node is its own predecessor's right child.
            var right = node.Right;
            node.Left = null;
            tail.Right = node;

            return VisitAndRelink(right, node);
        }
    }

    public static BinaryTreeNode<int>? IncreasingBstByInOrderHooks(BinaryTreeNode<int>? root)
    {
        var dummy = new BinaryTreeNode<int>(DummyHeadValue);
        State.Tail.Value = dummy;

        InOrderTraversal.Walk<int, RelinkHooks>(root);

        return dummy.Right;
    }

    private readonly struct RelinkHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            node.Left = null;
            State.Tail.Value!.Right = node;
            State.Tail.Value = node;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Tail = new();
    }
}
