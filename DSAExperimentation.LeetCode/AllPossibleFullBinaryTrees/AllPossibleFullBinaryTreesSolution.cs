using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.AllPossibleFullBinaryTrees;

// LeetCode 894. All Possible Full Binary Trees: a full binary tree (every node has 0
// or 2 children) with n nodes splits into a 1-node root plus a left subtree of i
// nodes and a right subtree of (n-1-i) nodes for every odd i - every combination of a
// full binary tree generated for i and one generated for (n-1-i) forms one distinct
// result, built directly out of this repo's own BinaryTreeNode<int>, the same "every
// combination of a left/right generation" composition UniqueBinarySearchTreesII
// already uses for LeetCode 95.
//
// The two strategies differ only in whether a repeated node-count subproblem - e.g.
// 5 is reached both directly and again nested inside larger splits - is regenerated
// from scratch each time or solved once and shared via Memoizer. The state is a
// single node count rather than LC 95's (start,end) range, since a full binary tree's
// shape never depends on the values it carries (every node is 0).
internal static class AllPossibleFullBinaryTreesSolution
{
    // A full binary tree only exists for odd node counts, so evenness is checked with
    // this divisor and left-subtree sizes are stepped by it to stay odd.
    private const int NodeCountParityDivisor = 2;

    // Textbook baseline: plain split-based recursion, re-deriving any node count
    // reached from more than one parent call.
    public static List<BinaryTreeNode<int>?> AllPossibleFbtByPlainRecursion(int n) =>
        BuildFullBinaryTrees(n, AllPossibleFbtByPlainRecursion);

    // Identical recurrence, driven top-down through Memoizer keyed by the node count,
    // so a count reached from multiple parents is generated once and its subtree
    // objects shared across every caller.
    public static List<BinaryTreeNode<int>?> AllPossibleFbtByMemoizedNodeCount(int n) =>
        Memoizer.Memoize<int, List<BinaryTreeNode<int>?>>(n, Recurrence);

    private static List<BinaryTreeNode<int>?> Recurrence(
        int n,
        Func<int, List<BinaryTreeNode<int>?>> generate) =>
        BuildFullBinaryTrees(n, generate);

    // Shared recurrence body for both strategies: they differ only in how sub-counts
    // are generated (plain self-recursion vs. a memoized delegate), so that single
    // point of variation is passed in as `generate`.
    private static List<BinaryTreeNode<int>?> BuildFullBinaryTrees(
        int n,
        Func<int, List<BinaryTreeNode<int>?>> generate)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (n % NodeCountParityDivisor == 0)
        {
            return trees;
        }

        if (n == 1)
        {
            trees.Add(new BinaryTreeNode<int>(0));
            return trees;
        }

        for (var leftCount = 1; leftCount < n; leftCount += NodeCountParityDivisor)
        {
            AppendSplits(trees, generate(leftCount), generate(n - 1 - leftCount));
        }

        return trees;
    }

    private static void AppendSplits(
        List<BinaryTreeNode<int>?> trees,
        List<BinaryTreeNode<int>?> lefts,
        List<BinaryTreeNode<int>?> rights)
    {
        foreach (var left in lefts)
        {
            foreach (var right in rights)
            {
                trees.Add(new BinaryTreeNode<int>(0) { Left = left, Right = right });
            }
        }
    }
}
