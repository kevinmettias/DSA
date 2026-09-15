using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.UniqueBinarySearchTreesII;

// LeetCode 95. Unique Binary Search Trees II: build every structurally
// distinct BST storing values 1..n. For each candidate root k in [start,end],
// every combination of a left subtree generated from [start,k-1] and a right
// subtree generated from [k+1,end] forms one distinct tree - built directly
// out of this repo's own BinaryTreeNode<int> (the "construct nodes ad hoc via
// object initializers" style ConvertSortedArrayToBinarySearchTree already
// uses), unlike UniqueBinarySearchTreesSolution's Insert-based
// BinarySearchTree<TValue>, whose insertion order only ever produces ONE
// shape per order, not every shape.
//
// The two strategies differ only in whether a repeated (start,end) sub-range
// - e.g. for n=4, (3,4) is reached both directly as root=2's right subtree
// and again nested inside root=1's right subtree (2,4) - is regenerated from
// scratch each time (call count grows as 3^n) or solved once and shared via
// Memoizer.
internal static class UniqueBinarySearchTreesIISolution
{
    // Textbook baseline: plain left-root-right recursion, re-deriving any
    // sub-range reached from more than one parent call.
    public static List<BinaryTreeNode<int>?> GenerateTreesByPlainRecursion(int n) =>
        GeneratePlain(1, n);

    // Identical recurrence, driven top-down through Memoizer keyed by the
    // (start,end) range, so a range reached from multiple parents is
    // generated once and its subtree objects shared across every caller.
    public static List<BinaryTreeNode<int>?> GenerateTreesByMemoizedRange(int n) =>
        Memoizer.Memoize<(int Start, int End), List<BinaryTreeNode<int>?>>((1, n), new EveryTreeOverRange());

    // The recurrence, named: a range of values is one tree per candidate root, each
    // root pairing every tree over its left sub-range with every tree over its right
    // one, and an inverted range is the empty-tree base case. A range reached from
    // more than one parent is solved once by the memo run, not re-derived per parent.
    private sealed class EveryTreeOverRange : IRecurrence<(int Start, int End), List<BinaryTreeNode<int>?>>
    {
        /// <inheritdoc/>
        public List<BinaryTreeNode<int>?> Replay(
            (int Start, int End) range,
            IRecurrence<(int Start, int End), List<BinaryTreeNode<int>?>> rest)
        {
            var trees = new List<BinaryTreeNode<int>?>();

            if (range.Start > range.End)
            {
                trees.Add(null);
                return trees;
            }

            for (var root = range.Start; root <= range.End; root++)
            {
                var lefts = rest.Replay((range.Start, root - 1), rest);
                var rights = rest.Replay((root + 1, range.End), rest);
                AppendCombinations(trees, root, lefts, rights);
            }

            return trees;
        }
    }

    private static List<BinaryTreeNode<int>?> GeneratePlain(int start, int end)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (start > end)
        {
            trees.Add(null);
            return trees;
        }

        for (var root = start; root <= end; root++)
        {
            var lefts = GeneratePlain(start, root - 1);
            var rights = GeneratePlain(root + 1, end);
            AppendCombinations(trees, root, lefts, rights);
        }

        return trees;
    }

    private static void AppendCombinations(
        List<BinaryTreeNode<int>?> trees,
        int root,
        List<BinaryTreeNode<int>?> lefts,
        List<BinaryTreeNode<int>?> rights)
    {
        foreach (var left in lefts)
        {
            foreach (var right in rights)
            {
                trees.Add(new BinaryTreeNode<int>(root) { Left = left, Right = right });
            }
        }
    }
}
