using DSAExperimentation.LeetCode.ShortestDistanceAfterRoadAdditionQueriesII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestDistanceAfterRoadAdditionQueriesII;

// Harness only. Both strategies are
// ShortestDistanceAfterRoadAdditionQueriesIISolution's - this file just pins them to
// LeetCode's published examples, including the second one (Example 2) where a later
// query is fully nested inside an earlier one and must leave the answer unchanged.
public sealed class ShortestDistanceAfterRoadAdditionQueriesIITests
{
    public static TheoryData<int, int[][], int[]> Examples =>
        new()
        {
            { 5, new[] { new[] { 2, 4 }, new[] { 0, 2 }, new[] { 0, 4 } }, new[] { 3, 2, 1 } },
            { 4, new[] { new[] { 0, 3 }, new[] { 0, 2 } }, new[] { 1, 1 } },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestDistancesByAdjacencyBfs_LeetCodeExamples_ReturnsShortestDistanceAfterEachQuery(
        int n, int[][] queries, int[] expected) =>
        Assert.Equal(expected, ShortestDistanceAfterRoadAdditionQueriesIISolution.ShortestDistancesByAdjacencyBfs(n, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestDistancesByIntervalSet_LeetCodeExamples_ReturnsShortestDistanceAfterEachQuery(
        int n, int[][] queries, int[] expected) =>
        Assert.Equal(expected, ShortestDistanceAfterRoadAdditionQueriesIISolution.ShortestDistancesByIntervalSet(n, queries));
}
