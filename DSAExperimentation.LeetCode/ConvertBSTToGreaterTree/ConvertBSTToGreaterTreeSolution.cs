using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.ConvertBSTToGreaterTree;

// LeetCode 538. Convert BST to Greater Tree: every node's value becomes the sum of
// itself and everything strictly greater in the tree, mutated in place.
//
// InOrderTraversal only walks ascending (left, visit, right - see its own doc
// comment on why it's hardwired that direction, not generic over it), so the
// composed strategy below runs two ascending InOrderTraversal/IInOrderHooks passes
// over the same BinaryTreeNode<int> tree instead of a single hand-rolled
// right-to-left walk: pass 1 collects values in ascending order, a suffix sum over
// that ascending list gives each rank's "sum of everything >= it," and pass 2 walks
// ascending again reassigning node.Value from that precomputed sum by rank. This is
// exactly the composition BinaryTreeNode<TValue>'s own doc comment names as the
// reason Value is settable in the first place.
//
// The baseline, ConvertByReverseInOrder, is the textbook single-pass recursive
// reverse in-order walk (right, visit+accumulate, left) - not an asymptotic win over
// the composed version (both are O(n) time) but it needs no second array and no
// second full walk, which is the real cost InOrderTraversal's missing reverse
// direction imposes on the composed strategy.
internal static class ConvertBSTToGreaterTreeSolution
{
    // The textbook baseline: a hand-rolled recursive reverse in-order walk
    // accumulating a running sum. Deliberately written without this repo's
    // traversal primitives - only the tree it is handed is a repo type.
    public static BinaryTreeNode<int>? ConvertByReverseInOrder(BinaryTreeNode<int>? root)
    {
        var runningSum = 0;
        VisitReverseInOrder(root, ref runningSum);
        return root;

        static void VisitReverseInOrder(BinaryTreeNode<int>? node, ref int runningSum)
        {
            if (node is null)
            {
                return;
            }

            VisitReverseInOrder(node.Right, ref runningSum);
            runningSum += node.Value;
            node.Value = runningSum;
            VisitReverseInOrder(node.Left, ref runningSum);
        }
    }

    // This repo's own ascending-only InOrderTraversal, composed into the answer via
    // two passes since InOrderTraversal cannot walk in reverse.
    public static BinaryTreeNode<int>? ConvertByInOrderHooks(BinaryTreeNode<int>? root)
    {
        var ascending = CollectAscendingValues(root);
        var suffixSums = BuildSuffixSums(ascending);

        return ApplySuffixSums(root, suffixSums);
    }

    // Pass 1: the ascending InOrderTraversal collects every node value in sorted order.
    private static List<int> CollectAscendingValues(BinaryTreeNode<int>? root)
    {
        var ascending = new List<int>();
        InOrderTraversal.Walk(root, new CollectHooks(ascending));

        return ascending;
    }

    // Turns the ascending values into, for each rank, the sum of every value at or above it.
    private static int[] BuildSuffixSums(List<int> ascending)
    {
        var suffixSums = new int[ascending.Count];
        var runningSum = 0;

        for (var i = ascending.Count - 1; i >= 0; i--)
        {
            runningSum += ascending[i];
            suffixSums[i] = runningSum;
        }

        return suffixSums;
    }

    // Pass 2: the same ascending walk reassigns each node by rank from the precomputed sums.
    private static BinaryTreeNode<int>? ApplySuffixSums(BinaryTreeNode<int>? root, int[] suffixSums)
    {
        InOrderTraversal.Walk(root, new AssignHooks(suffixSums));

        return root;
    }

    private readonly struct CollectHooks(List<int> values) : IInOrderHooks<int>
    {
        public void Visit(BinaryTreeNode<int> node, int depth) => values.Add(node.Value);
    }

    // Gives each node, in ascending order, the sum at its rank. The rank is the hook's own
    // count, so the struct is mutable by design.
    private struct AssignHooks(int[] greaterSums) : IInOrderHooks<int>
    {
        private int _index;

        public void Visit(BinaryTreeNode<int> node, int depth)
        {
            node.Value = greaterSums[_index];
            _index++;
        }
    }
}
