using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestMissingGeneticValueInEachSubtree;

// LeetCode 2003. Smallest Missing Genetic Value in Each Subtree: only the node
// holding genetic value 1 (and its ancestors up to the root) can possibly answer
// anything other than 1 - every other subtree can never contain value 1, so its mex
// is trivially 1. Walking from that node up to the root, each step DFS's only the
// *newly* reachable subtree portion (this repo's own DepthFirstSearch.Traverse,
// the same successor-skip technique MaximumGeneticDifferenceQueryTests already uses
// for "process the whole parent-array family tree exactly once while walking an
// ancestor chain"), folding each node's value into a running
// DataStructures.Set<int> and advancing a monotonically non-decreasing mex
// counter - the standard O(n) solution, not an O(n^2) resubtree-scan-per-node
// brute force.
public sealed partial class SmallestMissingGeneticValueInEachSubtreeTests
{
    [Fact]
    public void SmallestMissingValueInEachSubtree_BranchingTreeWithValueOneOnABranch_ReturnsPerNodeMex()
    {
        // 0(val=5) -> {1(val=1), 2(val=3)}; 1 -> 3(val=2)
        int[] parents = [-1, 0, 0, 1];
        int[] nums = [5, 1, 3, 2];

        var actual = SmallestMissingValueInEachSubtree(parents, nums);
        Assert.Equal([4, 3, 1, 1], actual);
    }

    [Fact]
    public void SmallestMissingValueInEachSubtree_NoNodeHoldsValueOne_EveryAnswerIsOne()
    {
        // chain 0 -> 1 -> 2, none of them carry genetic value 1
        int[] parents = [-1, 0, 1];
        int[] nums = [10, 20, 30];

        var actual = SmallestMissingValueInEachSubtree(parents, nums);
        Assert.Equal([1, 1, 1], actual);
    }

    private static int[] SmallestMissingValueInEachSubtree(int[] parents, int[] nums)
    {
        var answers = new int[parents.Length];
        Array.Fill(answers, 1);

        var nodeWithValueOne = Array.IndexOf(nums, 1);
        if (nodeWithValueOne == -1)
        {
            return answers;
        }

        var context = new SubtreeMexContext(BuildChildren(parents), nums, new Set<int>(), answers, parents);
        var mex = 1;
        var skip = -1;
        var node = nodeWithValueOne;

        while (node != -1)
        {
            (mex, skip, node) = AdvanceAncestor(node, skip, mex, context);
        }

        return answers;
    }

    private static (int Mex, int Skip, int Node) AdvanceAncestor(int node, int skip, int mex, SubtreeMexContext context)
    {
        var nextMex = AdvanceMex(node, skip, mex, context);
        context.Answers[node] = nextMex;
        return (nextMex, node, context.Parents[node]);
    }

    private static int AdvanceMex(int node, int skip, int mex, SubtreeMexContext context)
    {
        var newlyVisited = DepthFirstSearch.Traverse(node, current => context.Children[current].Where(c => c != skip));

        foreach (var visited in newlyVisited)
        {
            context.GeneValues.TryAdd(context.Nums[visited]);
        }

        while (context.GeneValues.Has(mex))
        {
            mex++;
        }

        return mex;
    }

    private readonly record struct SubtreeMexContext(List<int>[] Children, int[] Nums, Set<int> GeneValues, int[] Answers, int[] Parents);

    private static List<int>[] BuildChildren(int[] parents)
    {
        var children = new List<int>[parents.Length];
        for (var i = 0; i < parents.Length; i++)
        {
            children[i] = [];
        }

        for (var i = 0; i < parents.Length; i++)
        {
            if (parents[i] != -1)
            {
                children[parents[i]].Add(i);
            }
        }

        return children;
    }
}
