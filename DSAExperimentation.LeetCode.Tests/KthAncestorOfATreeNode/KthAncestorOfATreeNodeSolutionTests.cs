using DSAExperimentation.LeetCode.KthAncestorOfATreeNode;

namespace DSAExperimentation.LeetCode.Tests.KthAncestorOfATreeNode;

// Harness only: both strategies are KthAncestorOfATreeNodeSolution's. The parent
// array below is LeetCode's published example - a complete binary tree on seven
// nodes - plus a straight chain, so both the "chain runs out" and the "deepest
// possible node" cases are asserted against each strategy.
//
//   node:   0
//          / \
//         1   2
//        / \ / \
//       3  4 5  6
//
// The last block of queries runs on the deepest tree LeetCode allows: a chain of
// 5 * 10^4 nodes, where node v's parent is v - 1, so its kth ancestor is v - k
// while k <= v and -1 past that. A strategy that stores every node's whole root
// path needs about 1.25 * 10^9 ints there.
public sealed partial class KthAncestorOfATreeNodeSolutionTests
{
    // LeetCode's largest n.
    private const int LongestChainLength = 50_000;

    // 2^16 = 65,536 is the smallest power of two at least 5 * 10^4, so sixteen
    // levels spell every step count up to 49,999 in binary.
    private const int LongestChainLevels = 16;

    private static readonly int[] LongestChain = ChainOf(LongestChainLength);

    public static TheoryData<int[], int, int, int> Examples =>
        new()
        {
            // LeetCode's published example: getKthAncestor(3, 1), (5, 2), (6, 3).
            { [-1, 0, 0, 1, 1, 2, 2], 3, 1, 1 },
            { [-1, 0, 0, 1, 1, 2, 2], 5, 2, 0 },
            { [-1, 0, 0, 1, 1, 2, 2], 6, 3, -1 },

            // The rest of that same tree, one query per remaining shape of answer.
            { [-1, 0, 0, 1, 1, 2, 2], 4, 1, 1 },
            { [-1, 0, 0, 1, 1, 2, 2], 4, 2, 0 },
            { [-1, 0, 0, 1, 1, 2, 2], 6, 1, 2 },
            { [-1, 0, 0, 1, 1, 2, 2], 1, 1, 0 },
            { [-1, 0, 0, 1, 1, 2, 2], 1, 2, -1 },

            // The root has no ancestors at all.
            { [-1, 0, 0], 0, 1, -1 },
            { [-1, 0, 0, 1, 1, 2, 2], 0, 1, -1 },

            // A straight chain: the deepest node reaches the root at k = depth and
            // runs out one step later, at k = n, the largest k LeetCode allows.
            { [-1, 0, 1, 2, 3], 4, 1, 3 },
            { [-1, 0, 1, 2, 3], 4, 4, 0 },
            { [-1, 0, 1, 2, 3], 4, 5, -1 },

            // Parents numbered after their children, which LeetCode allows: 3's
            // parent is 1, 1's is 2 and 2's is the root.
            { [-1, 2, 0, 1], 3, 1, 1 },
            { [-1, 2, 0, 1], 3, 2, 2 },
            { [-1, 2, 0, 1], 3, 3, 0 },
            { [-1, 2, 0, 1], 3, 4, -1 },

            // A single-node tree, the smallest input LeetCode's constraints allow.
            { [-1], 0, 1, -1 },
        };

    // Queries on LongestChain: node, k, and v - k, or -1 once k passes v.
    public static TheoryData<int, int, int> LongestChainQueries =>
        new()
        {
            { 49_999, 1, 49_998 },

            // The deepest node reaches the root in 49,999 steps, the most any node
            // can take, and runs out at k = n.
            { 49_999, 49_999, 0 },
            { 49_999, 50_000, -1 },

            // 2^15 alone, the highest level, then every level below it at once.
            { 49_999, 32_768, 17_231 },
            { 49_999, 32_767, 17_232 },

            { 25_000, 12_345, 12_655 },
            { 25_000, 25_001, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetKthAncestorByParentWalk_LeetCodeExamples_ReturnsAncestorOrMinusOne(
        int[] parent, int node, int ancestorSteps, int expected)
    {
        var ancestor = KthAncestorOfATreeNodeSolution.GetKthAncestorByParentWalk(parent, node, ancestorSteps);

        Assert.Equal(expected, ancestor);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetKthAncestorByBinaryLifting_LeetCodeExamples_ReturnsAncestorOrMinusOne(
        int[] parent, int node, int ancestorSteps, int expected)
    {
        var ancestor = KthAncestorOfATreeNodeSolution.GetKthAncestorByBinaryLifting(parent, node, ancestorSteps);

        Assert.Equal(expected, ancestor);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildAncestorJumps_LeetCodeExamples_AnswersFromOnePrecomputedTable(
        int[] parent, int node, int ancestorSteps, int expected)
    {
        var jumps = KthAncestorOfATreeNodeSolution.BuildAncestorJumps(parent);
        var ancestor = KthAncestorOfATreeNodeSolution.GetKthAncestorByBinaryLifting(jumps, node, ancestorSteps);

        Assert.Equal(parent.Length, jumps.NodeCount);
        Assert.Equal(expected, ancestor);
    }

    [Theory]
    [MemberData(nameof(LongestChainQueries))]
    public void GetKthAncestorByParentWalk_LeetCodesLongestChain_ReturnsAncestorOrMinusOne(
        int node, int ancestorSteps, int expected)
    {
        var ancestor = KthAncestorOfATreeNodeSolution.GetKthAncestorByParentWalk(LongestChain, node, ancestorSteps);

        Assert.Equal(expected, ancestor);
    }

    [Theory]
    [MemberData(nameof(LongestChainQueries))]
    public void GetKthAncestorByBinaryLifting_LeetCodesLongestChain_ReturnsAncestorOrMinusOne(
        int node, int ancestorSteps, int expected)
    {
        var ancestor = KthAncestorOfATreeNodeSolution.GetKthAncestorByBinaryLifting(LongestChain, node, ancestorSteps);

        Assert.Equal(expected, ancestor);
    }

    [Theory]
    [MemberData(nameof(LongestChainQueries))]
    public void BuildAncestorJumps_LeetCodesLongestChain_HoldsSixteenLevels(
        int node, int ancestorSteps, int expected)
    {
        var jumps = KthAncestorOfATreeNodeSolution.BuildAncestorJumps(LongestChain);
        var ancestor = KthAncestorOfATreeNodeSolution.GetKthAncestorByBinaryLifting(jumps, node, ancestorSteps);

        Assert.Equal(LongestChainLevels, jumps.Levels);
        Assert.Equal(expected, ancestor);
    }

    // parent[v] = v - 1: the deepest tree a given node count admits.
    private static int[] ChainOf(int length) =>
        [.. Enumerable.Range(0, length).Select(node => node - 1)];
}
