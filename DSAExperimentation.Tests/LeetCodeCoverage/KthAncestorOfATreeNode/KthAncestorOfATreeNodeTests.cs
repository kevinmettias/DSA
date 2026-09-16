using DSAExperimentation.LeetCode.KthAncestorOfATreeNode;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthAncestorOfATreeNode;

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
public sealed partial class KthAncestorOfATreeNodeTests
{
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
            // runs out one step later.
            { [-1, 0, 1, 2, 3], 4, 1, 3 },
            { [-1, 0, 1, 2, 3], 4, 4, 0 },
            { [-1, 0, 1, 2, 3], 4, 5, -1 },

            // A single-node tree, the smallest input LeetCode's constraints allow.
            { [-1], 0, 1, -1 },
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
    public void GetKthAncestorByAncestorChains_LeetCodeExamples_ReturnsAncestorOrMinusOne(
        int[] parent, int node, int ancestorSteps, int expected)
    {
        var ancestor = KthAncestorOfATreeNodeSolution.GetKthAncestorByAncestorChains(parent, node, ancestorSteps);

        Assert.Equal(expected, ancestor);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildAncestorChains_LeetCodeExamples_AnswersFromOnePrecomputedTable(
        int[] parent, int node, int ancestorSteps, int expected)
    {
        var chains = KthAncestorOfATreeNodeSolution.BuildAncestorChains(parent);
        var ancestor = KthAncestorOfATreeNodeSolution.GetKthAncestorByAncestorChains(chains, node, ancestorSteps);

        Assert.Equal(parent.Length, chains.Count);
        Assert.Equal(expected, ancestor);
    }
}
