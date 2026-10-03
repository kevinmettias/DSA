using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.KthSmallestElementInABST;

// LeetCode 230. Kth Smallest Element in a BST: an in-order walk of a BST visits
// values in ascending order, so the kth value visited is the answer.
//
// Neither strategy early-exits once the kth value is found - IInOrderHooks.Visit
// has no such signal - so both walk the whole tree; the comparison is
// genuinely-distinct walk mechanics, not an asymptotic win.
internal static class KthSmallestElementInABSTSolution
{
    // The textbook approach: a hand-rolled recursive in-order walk, counting down
    // as each node is visited. Written without this repo's traversal engine - the
    // arm KthSmallestByInOrderTraversal below has to justify itself against.
    public static int KthSmallestByRecursiveWalk(BinaryTreeNode<int>? root, int rank)
    {
        var (_, result) = VisitByRecursiveWalk(root, rank, null);
        return result!.Value;
    }

    // This repo's own InOrderTraversal/IInOrderHooks composition - the same one
    // RecoverBinarySearchTreeSolution.RecoverByInOrderHooks uses. The running rank and
    // the result live in the hook, which the walk hands back.
    public static int KthSmallestByInOrderTraversal(BinaryTreeNode<int>? root, int rank)
        => InOrderTraversal.Walk(root, new RankHooks(rank)).Result!.Value;

    // The hand-rolled in-order walk, carrying its two pieces of running state -
    // visits still owed and the value that emptied the count - through the recursion
    // rather than closing over them.
    private static (int Remaining, int? Result) VisitByRecursiveWalk(
        BinaryTreeNode<int>? node, int remaining, int? result)
    {
        if (node is null)
        {
            return (remaining, result);
        }

        (remaining, result) = VisitByRecursiveWalk(node.Left, remaining, result);

        if (result is null)
        {
            remaining--;

            if (remaining == 0)
            {
                result = node.Value;
            }
        }

        return VisitByRecursiveWalk(node.Right, remaining, result);
    }

    // Counts down to the rank-th visit and keeps the value that emptied the count; later visits
    // leave it alone. Mutable by design: the walk hands back the value it finished with.
    private struct RankHooks(int rank) : IInOrderHooks<int>
    {
        private int _remaining = rank;

        public int? Result { get; private set; }

        public void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (Result is not null)
            {
                return;
            }

            _remaining--;

            if (_remaining == 0)
            {
                Result = node.Value;
            }
        }
    }
}
