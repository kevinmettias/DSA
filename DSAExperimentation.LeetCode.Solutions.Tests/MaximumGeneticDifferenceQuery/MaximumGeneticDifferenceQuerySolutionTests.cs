using DSAExperimentation.LeetCode.Solutions.MaximumGeneticDifferenceQuery;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumGeneticDifferenceQuery;

// Harness only. Both strategies are MaximumGeneticDifferenceQuerySolution's - this
// file just pins them to the shapes the offline
// DFS has to get right that the two published cases do not exercise: a single-node
// tree, a straight chain, and a node queried twice.
public sealed partial class MaximumGeneticDifferenceQuerySolutionTests
{
    public static TheoryData<int[], int[][], int[]> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void MaxGeneticDifferenceByAncestorWalk_Cases_ReturnsBestXorPerQuery(
        int[] parents, int[][] queries, int[] expected)
    {
        var actual = MaximumGeneticDifferenceQuerySolution.MaxGeneticDifferenceByAncestorWalk(parents, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxGeneticDifferenceByBitTrieDfs_Cases_ReturnsBestXorPerQuery(
        int[] parents, int[][] queries, int[] expected)
    {
        var actual = MaximumGeneticDifferenceQuerySolution.MaxGeneticDifferenceByBitTrieDfs(parents, queries);

        Assert.Equal(expected, actual);
    }
}
