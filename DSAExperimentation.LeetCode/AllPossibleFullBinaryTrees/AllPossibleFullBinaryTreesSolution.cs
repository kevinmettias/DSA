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
    // reached from more than one parent call. The recurrence is the same named type
    // the memoized arm uses; here it is handed itself as `rest`, so every sub-count
    // is regenerated from scratch.
    public static List<BinaryTreeNode<int>?> AllPossibleFbtByPlainRecursion(int n) =>
        BuildFullBinaryTrees(n, new SplitAtEveryOddLeftCount());

    // Shared recurrence body for both strategies: they differ only in whether
    // sub-counts are generated through a cache, so that single point of variation is
    // the `generate` recurrence the sweep hands itself.
    private static List<BinaryTreeNode<int>?> BuildFullBinaryTrees(
        int n,
        IRecurrence<int, List<BinaryTreeNode<int>?>> generate)
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
            var lefts = generate.Replay(leftCount, generate);
            var rights = generate.Replay(n - 1 - leftCount, generate);
            AppendSplits(trees, lefts, rights);
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

    // Identical recurrence, driven top-down through Memoizer keyed by the node count,
    // so a count reached from multiple parents is generated once and its subtree
    // objects shared across every caller.
    public static List<BinaryTreeNode<int>?> AllPossibleFbtByMemoizedNodeCount(int n) =>
        Memoizer.Memoize<int, List<BinaryTreeNode<int>?>>(n, new SplitAtEveryOddLeftCount());

    // The recurrence, named: a full binary tree of n nodes is a 1-node root plus a
    // left subtree of every odd count below n and a right subtree making up the rest.
    // Whether a repeated count is regenerated or shared is the memo run's business,
    // not the rule's.
    private sealed class SplitAtEveryOddLeftCount : IRecurrence<int, List<BinaryTreeNode<int>?>>
    {
        public List<BinaryTreeNode<int>?> Replay(int n, IRecurrence<int, List<BinaryTreeNode<int>?>> rest) =>
            BuildFullBinaryTrees(n, rest);
    }
}
