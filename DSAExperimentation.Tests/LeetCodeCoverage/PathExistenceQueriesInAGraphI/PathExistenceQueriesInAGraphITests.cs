using DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathExistenceQueriesInAGraphI;

// Harness only. ProximityGroups reduces LC 3532's graph to its n-1 adjacent-pair
// edges and both strategies are PathExistenceQueriesInAGraphISolution's - this
// file just pins them to LeetCode's published examples.
public sealed class PathExistenceQueriesInAGraphITests
{
    public static TheoryData<int, int[], int, int[][], bool[]> Examples =>
        new()
        {
            {
                2, [1, 3], 1,
                [[0, 0], [0, 1]],
                [true, false]
            },
            {
                4, [2, 5, 6, 8], 2,
                [[0, 1], [0, 2], [1, 3], [2, 3]],
                [false, false, true, true]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PathExistenceQueriesByBruteForceBfs_LeetCodeExamples_ReturnsWhetherEachQueryPairIsConnected(
        int n, int[] nums, int maxDiff, int[][] queries, bool[] expected) =>
        Assert.Equal(
            expected,
            PathExistenceQueriesInAGraphISolution.PathExistenceQueriesByBruteForceBfs(n, nums, maxDiff, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PathExistenceQueriesByDisjointSet_LeetCodeExamples_ReturnsWhetherEachQueryPairIsConnected(
        int n, int[] nums, int maxDiff, int[][] queries, bool[] expected) =>
        Assert.Equal(
            expected,
            PathExistenceQueriesInAGraphISolution.PathExistenceQueriesByDisjointSet(n, nums, maxDiff, queries));
}
