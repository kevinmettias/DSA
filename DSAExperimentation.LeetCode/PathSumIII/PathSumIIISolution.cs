using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PathSumIII;

// LeetCode 437. Path Sum III: unlike PathSum/PathSumII, a qualifying path need
// not start at the root or end at a leaf, so a plain root-to-leaf walk doesn't
// apply directly.
internal static class PathSumIIISolution
{
    // The textbook answer: for every node, walk its entire downward subtree
    // summing paths that start there - O(n^2) on a balanced tree. Deliberately
    // written without this repo's primitives; it is the arm the composed
    // solution below has to justify itself against.
    public static int PathSumByDoubleDfs(BinaryTreeNode<int>? root, int target) =>
        CountFromEveryNode(root, target);

    private static int CountFromEveryNode(BinaryTreeNode<int>? node, int target)
    {
        if (node is null)
        {
            return 0;
        }

        return CountDownwardFrom(node, 0, target)
            + CountFromEveryNode(node.Left, target)
            + CountFromEveryNode(node.Right, target);
    }

    private static int CountDownwardFrom(BinaryTreeNode<int>? node, long runningSum, int target)
    {
        if (node is null)
        {
            return 0;
        }

        runningSum += node.Value;
        var matches = runningSum == target ? 1 : 0;

        return matches
            + CountDownwardFrom(node.Left, runningSum, target)
            + CountDownwardFrom(node.Right, runningSum, target);
    }

    // A single DFS carrying a running root-to-node sum plus this repo's own
    // HashMap<long,int> as a tally of ancestor prefix sums - the same
    // "count matches via a running-sum HashMap" idea TwoSum proves for a flat
    // array. At each node, the number of downward paths ending here summing to
    // target equals how many ancestor prefix sums equal (runningSum - target).
    // Each node's own tally entry is added before descending into its children
    // and removed again afterward (backtracking), so a sibling subtree never
    // sees it.
    public static int PathSumByPrefixSumHashMap(BinaryTreeNode<int>? root, int target)
    {
        var prefixSumCounts = new HashMap<long, int>();
        prefixSumCounts.Set(0, 1);

        return CountPaths(root, 0, target, prefixSumCounts);
    }

    private static int CountPaths(BinaryTreeNode<int>? node, long runningSum, int target, HashMap<long, int> prefixSumCounts)
    {
        if (node is null)
        {
            return 0;
        }

        runningSum += node.Value;
        var matches = CountMatchesEndingHere(runningSum, target, prefixSumCounts);

        IncrementPrefixSumCount(runningSum, prefixSumCounts);

        matches += CountPaths(node.Left, runningSum, target, prefixSumCounts);
        matches += CountPaths(node.Right, runningSum, target, prefixSumCounts);

        DecrementPrefixSumCount(runningSum, prefixSumCounts);

        return matches;
    }

    private static int CountMatchesEndingHere(long runningSum, int target, HashMap<long, int> prefixSumCounts) =>
        prefixSumCounts.TryGetValue(runningSum - target, out var count) ? count : 0;

    private static void IncrementPrefixSumCount(long runningSum, HashMap<long, int> prefixSumCounts)
    {
        var existing = prefixSumCounts.TryGetValue(runningSum, out var count) ? count : 0;
        prefixSumCounts.Set(runningSum, existing + 1);
    }

    // Backtracking step: undoes the increment IncrementPrefixSumCount made for
    // this node before returning to the parent call, so a sibling subtree never
    // sees it.
    private static void DecrementPrefixSumCount(long runningSum, HashMap<long, int> prefixSumCounts)
    {
        var afterChildren = prefixSumCounts.TryGetValue(runningSum, out var updated) ? updated : 0;

        if (afterChildren <= 1)
        {
            prefixSumCounts.TryRemove(runningSum);
        }
        else
        {
            prefixSumCounts.Set(runningSum, afterChildren - 1);
        }
    }
}
