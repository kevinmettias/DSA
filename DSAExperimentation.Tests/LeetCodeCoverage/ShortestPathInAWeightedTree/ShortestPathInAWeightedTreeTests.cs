using DSAExperimentation.LeetCode.ShortestPathInAWeightedTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathInAWeightedTree;

// Harness only. Both strategies are ShortestPathInAWeightedTreeSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class ShortestPathInAWeightedTreeTests
{
    public static TheoryData<int, int[][], int[][], int[]> Examples =>
        new()
        {
            {
                2,
                [[1, 2, 7]],
                [[2, 2], [1, 1, 2, 4], [2, 2]],
                [7, 4]
            },
            {
                3,
                [[1, 2, 2], [1, 3, 4]],
                [[2, 1], [2, 3], [1, 1, 3, 7], [2, 2], [2, 3]],
                [0, 4, 2, 7]
            },
            {
                4,
                [[1, 2, 2], [2, 3, 1], [3, 4, 5]],
                [[2, 4], [2, 3], [1, 2, 3, 3], [2, 2], [2, 3]],
                [8, 3, 2, 5]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestPathQueriesByBruteForceBfs_LeetCodeExamples_ReturnsDistancesFromRoot(
        int nodeCount, int[][] edges, int[][] queries, int[] expected)
    {
        var actual = ShortestPathInAWeightedTreeSolution.ShortestPathQueriesByBruteForceBfs(nodeCount, edges, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestPathQueriesByEulerFenwick_LeetCodeExamples_ReturnsDistancesFromRoot(
        int nodeCount, int[][] edges, int[][] queries, int[] expected)
    {
        var actual = ShortestPathInAWeightedTreeSolution.ShortestPathQueriesByEulerFenwick(nodeCount, edges, queries);

        Assert.Equal(expected, actual);
    }
}
