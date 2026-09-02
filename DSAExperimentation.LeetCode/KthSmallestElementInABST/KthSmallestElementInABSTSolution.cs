using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.KthSmallestElementInABST;

// LeetCode 230. Kth Smallest Element in a BST: an in-order walk of a BST visits
// values in ascending order, so the kth value visited is the answer.
//
// Neither strategy early-exits once the kth value is found - IInOrderHooks.Visit
// has no such signal (see InOrderTraversal's own doc note) - so both walk the whole
// tree; the comparison is genuinely-distinct walk mechanics, not an asymptotic win.
internal static class KthSmallestElementInABSTSolution
{
    // The textbook approach: a hand-rolled recursive in-order walk, counting down
    // as each node is visited. Written without this repo's traversal engine - the
    // arm KthSmallestByInOrderTraversal below has to justify itself against.
    public static int KthSmallestByRecursiveWalk(BinaryTreeNode<int>? root, int k)
    {
        var remaining = k;
        int? result = null;

        void Visit(BinaryTreeNode<int>? node)
        {
            if (node is null)
            {
                return;
            }

            Visit(node.Left);

            if (result is null)
            {
                remaining--;

                if (remaining == 0)
                {
                    result = node.Value;
                }
            }

            Visit(node.Right);
        }

        Visit(root);
        return result!.Value;
    }

    // This repo's own InOrderTraversal/IInOrderHooks composition - the same one
    // RecoverBinarySearchTreeSolution.RecoverByInOrderHooks uses. Hooks are static,
    // so the running rank and result live in AsyncLocal state alongside the walk.
    public static int KthSmallestByInOrderTraversal(BinaryTreeNode<int>? root, int k)
    {
        State.Remaining.Value = k;
        State.Result.Value = null;

        InOrderTraversal.Walk<int, RankHooks>(root);

        return State.Result.Value!.Value;
    }

    private readonly struct RankHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Result.Value is not null)
            {
                return;
            }

            State.Remaining.Value--;

            if (State.Remaining.Value == 0)
            {
                State.Result.Value = node.Value;
            }
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<int> Remaining = new();
        public static readonly AsyncLocal<int?> Result = new();
    }
}
