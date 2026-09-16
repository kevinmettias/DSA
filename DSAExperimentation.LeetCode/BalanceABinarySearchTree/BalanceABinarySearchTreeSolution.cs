using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.BalanceABinarySearchTree;

// LeetCode 1382. Balance a Binary Search Tree: return a height-balanced BST
// holding the same values as the given one, no matter how skewed the input was.
//
// Both strategies are the same two steps - collect the values in ascending order,
// then rebuild by always splitting at the midpoint (exactly
// ConvertSortedArrayToBinarySearchTreeSolution.BuildByMidpointRecursion's
// reasoning: the two halves can differ in length by at most one, every time). They
// differ only in how the ascending order is obtained.
//
// BalanceByInOrderTraversal composes this repo's own InOrderTraversal/IInOrderHooks
// - the same composition KthSmallestElementInABSTSolution and
// FindModeInBinarySearchTreeSolution use - visiting every node exactly once, O(n),
// into a DynamicArray<int> rather than a BCL List<int>.
//
// BalanceByRepeatedKthSmallest is the textbook baseline that has to justify itself
// against it: a naive way to collect an already-BST's values in sorted order
// without trusting its shape is to re-derive the k-th smallest from scratch for
// every rank k = 1..n, each call its own O(n) in-order walk, O(n^2) overall. Its
// internals are deliberately BCL only; it is what you would write without this
// repo. Before this migration it lived as untested scaffolding inside
// BalanceABinarySearchTreeBenchmarks, so nothing asserted the arm the composed
// strategy is measured against.
internal static class BalanceABinarySearchTreeSolution
{

    public static BinaryTreeNode<int>? BalanceByInOrderTraversal(BinaryTreeNode<int>? root)
    {
        State.Sorted.Value = new DynamicArray<int>();

        InOrderTraversal.Walk<int, CollectHooks>(root);

        var sorted = State.Sorted.Value;
        return BuildFromDynamicArray(sorted, 0, sorted.Count - 1);
    }

    public static BinaryTreeNode<int>? BalanceByRepeatedKthSmallest(BinaryTreeNode<int>? root)
    {
        var count = CountNodes(root);
        var sorted = new int[count];

        for (var k = 1; k <= count; k++)
        {
            sorted[k - 1] = FindKthSmallest(root, k);
        }

        return BuildFromArray(sorted, 0, count - 1);
    }

    private static int FindKthSmallest(BinaryTreeNode<int>? root, int rank)
    {
        var remaining = rank;
        var result = 0;
        VisitInOrder(root);
        return result;

        void VisitInOrder(BinaryTreeNode<int>? node)
        {
            if (node is null || remaining == 0)
            {
                return;
            }

            VisitInOrder(node.Left);

            if (remaining == 0)
            {
                return;
            }

            remaining--;

            if (remaining == 0)
            {
                result = node.Value;
                return;
            }

            VisitInOrder(node.Right);
        }
    }

    private static int CountNodes(BinaryTreeNode<int>? node)
        => node is null ? 0 : CountSubtree(node);

    // The node itself plus every node its two subtrees hold.
    private static int CountSubtree(BinaryTreeNode<int> node)
        => 1 + CountNodes(node.Left) + CountNodes(node.Right);

    private static BinaryTreeNode<int>? BuildFromArray(int[] sorted, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

        return new BinaryTreeNode<int>(sorted[mid])
        {
            Left = BuildFromArray(sorted, low, mid - 1),
            Right = BuildFromArray(sorted, mid + 1, high),
        };
    }

    private static BinaryTreeNode<int>? BuildFromDynamicArray(DynamicArray<int> sorted, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

        return new BinaryTreeNode<int>(sorted.Get(mid))
        {
            Left = BuildFromDynamicArray(sorted, low, mid - 1),
            Right = BuildFromDynamicArray(sorted, mid + 1, high),
        };
    }

    // Hooks are static, so the collected values live in AsyncLocal state alongside
    // the walk - the same shape KthSmallestElementInABSTSolution's RankHooks uses.
    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Sorted.Value!.Add(node.Value);
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Sorted = new();
    }
}
