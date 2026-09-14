using DSAExperimentation.LeetCode.MaximumGeneticDifferenceQuery;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumGeneticDifferenceQuery;

// Harness only. Both strategies are MaximumGeneticDifferenceQuerySolution's - this
// file just pins them to LeetCode's published examples plus the shapes the offline
// DFS has to get right that the two published cases do not exercise: a single-node
// tree, a straight chain, and a node queried twice.
public sealed class MaximumGeneticDifferenceQueryTests
{
    public static TheoryData<int[], int[][], int[]> Examples =>
        new()
        {
            // LC example 1: node 0 (root) -> 1 -> {2, 3}.
            { [-1, 0, 1, 1], [[0, 2], [3, 2], [2, 5]], [2, 3, 7] },

            // LC example 2: root=2 -> {3, 7}; 3 -> {0}; 7 -> {1, 5}; 0 -> {4, 6}.
            { [3, 7, -1, 2, 0, 7, 0, 2], [[4, 6], [1, 15], [0, 5]], [6, 14, 7] },

            // A lone root, queried twice: the only candidate is the root's own id, 0.
            { [-1], [[0, 0], [0, 5]], [0, 5] },

            // A straight chain 0 -> 1 -> 2, the shape that makes every ancestor set a
            // prefix of the previous one.
            { [-1, 0, 1], [[2, 1], [1, 2], [0, 3]], [3, 3, 3] },

            // A branching tree with two queries parked on the same subtree, so the
            // DFS has to have removed node 3's id again before answering node 4's.
            { [-1, 0, 0, 1, 1], [[3, 10], [4, 1], [2, 7], [0, 0]], [11, 5, 7, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxGeneticDifferenceByAncestorWalk_LeetCodeExamples_ReturnsBestXorPerQuery(
        int[] parents, int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            MaximumGeneticDifferenceQuerySolution.MaxGeneticDifferenceByAncestorWalk(parents, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxGeneticDifferenceByBitTrieDfs_LeetCodeExamples_ReturnsBestXorPerQuery(
        int[] parents, int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            MaximumGeneticDifferenceQuerySolution.MaxGeneticDifferenceByBitTrieDfs(parents, queries));
}
