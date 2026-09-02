using DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathExistenceQueriesInAGraphII;

// Harness only: both strategies live in PathExistenceQueriesInAGraphIISolution and
// are asserted against the same examples, so a failure names the strategy that
// broke. Example 2's [2,3] query and example 3's last two queries are the
// unreachable (-1) cases; example 2's [0,2] is the only query in this set that
// needs more than a single hop.
public sealed class PathExistenceQueriesInAGraphIITests
{
    public static TheoryData<int, int[], int, int[][], int[]> Examples =>
        new()
        {
            { 5, [1, 8, 3, 4, 2], 3, [[0, 3], [2, 4]], [1, 1] },
            { 5, [5, 3, 1, 9, 10], 2, [[0, 1], [0, 2], [2, 3], [4, 3]], [1, 2, -1, 1] },
            { 3, [3, 6, 1], 1, [[0, 0], [0, 1], [1, 2]], [0, -1, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistancesByRangeBfs_LeetCodeExamples_ReturnsShortestHopCountPerQuery(
        int n, int[] nums, int maxDiff, int[][] queries, int[] expected) =>
        Assert.Equal(expected, PathExistenceQueriesInAGraphIISolution.MinDistancesByRangeBfs(n, nums, maxDiff, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistancesByBinaryLifting_LeetCodeExamples_ReturnsShortestHopCountPerQuery(
        int n, int[] nums, int maxDiff, int[][] queries, int[] expected) =>
        Assert.Equal(expected, PathExistenceQueriesInAGraphIISolution.MinDistancesByBinaryLifting(n, nums, maxDiff, queries));
}
