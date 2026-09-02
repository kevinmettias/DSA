using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSumIII;

// LeetCode 437. Path Sum III: unlike PathSum/PathSumII, a qualifying path need
// not start at the root or end at a leaf, so AllRootToLeafPaths doesn't apply
// directly. Composes this repo's BinaryTreeNode<int> with its own
// HashMap<long,int> as a running root-to-node prefix-sum tally (the same
// "count matches via a running-sum HashMap" idea TwoSumTests proves for a flat
// array): at each node, the number of downward paths ending here summing to
// target equals how many ancestor prefix sums equal (runningSum - target).
// Each node's own tally entry is added before descending into its children and
// removed again afterward (backtracking), so a sibling subtree never sees it.
public sealed partial class PathSumIIITests
{
    [Fact]
    public void PathSum_ClassicExample_ReturnsThree()
    {
        // 10
        // |-- 5
        // |   |-- 3
        // |   |   |-- 3
        // |   |   `-- -2
        // |   `-- 2
        // |       `-- 1
        // `-- -3
        //     `-- 11
        // Matching paths (sum 8): 5->3, 5->2->1, -3->11.
        var root = new BinaryTreeNode<int>(10)
        {
            Left = new BinaryTreeNode<int>(5)
            {
                Left = new BinaryTreeNode<int>(3)
                {
                    Left = new BinaryTreeNode<int>(3),
                    Right = new BinaryTreeNode<int>(-2),
                },
                Right = new BinaryTreeNode<int>(2)
                {
                    Right = new BinaryTreeNode<int>(1),
                },
            },
            Right = new BinaryTreeNode<int>(-3)
            {
                Right = new BinaryTreeNode<int>(11),
            },
        };

        var actual = PathSum(root, 8);
        Assert.Equal(3, actual);
    }

    [Fact]
    public void PathSum_NoDownwardPathMatchesTarget_ReturnsZero()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new(-2), Right = new(-3) };

        var actual = PathSum(root, 100);
        Assert.Equal(0, actual);
    }

    private static int PathSum(BinaryTreeNode<int>? root, int target)
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

    private static int CountMatchesEndingHere(long runningSum, int target, HashMap<long, int> prefixSumCounts)
        => prefixSumCounts.TryGetValue(runningSum - target, out var count) ? count : 0;

    private static void IncrementPrefixSumCount(long runningSum, HashMap<long, int> prefixSumCounts)
    {
        var existing = prefixSumCounts.TryGetValue(runningSum, out var count) ? count : 0;
        prefixSumCounts.Set(runningSum, existing + 1);
    }

    // Backtracking step: undoes the increment IncrementPrefixSumCount made for this
    // node before returning to the parent call, so a sibling subtree never sees it.
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
