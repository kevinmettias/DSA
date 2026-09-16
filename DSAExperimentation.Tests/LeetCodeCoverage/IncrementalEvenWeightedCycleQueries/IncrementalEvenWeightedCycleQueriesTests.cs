using DSAExperimentation.LeetCode.IncrementalEvenWeightedCycleQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IncrementalEvenWeightedCycleQueries;

// Harness only. Both strategies are IncrementalEvenWeightedCycleQueriesSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class IncrementalEvenWeightedCycleQueriesTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            { 3, [[0, 1, 1], [1, 2, 1], [0, 2, 1]], 2 },
            { 3, [[0, 1, 1], [1, 2, 1], [0, 2, 0]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountAddedEdgesByBruteForceBfs_LeetCodeExamples_ReturnsAcceptedEdgeCount(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = IncrementalEvenWeightedCycleQueriesSolution.CountAddedEdgesByBruteForceBfs(
            nodeCount, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountAddedEdgesByDisjointSetPrunedBfs_LeetCodeExamples_ReturnsAcceptedEdgeCount(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = IncrementalEvenWeightedCycleQueriesSolution.CountAddedEdgesByDisjointSetPrunedBfs(
            nodeCount, edges);

        Assert.Equal(expected, actual);
    }
}
