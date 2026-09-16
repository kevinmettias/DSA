using DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathExistenceQueriesInAGraphI;

// Harness only. ProximityGroups reduces LC 3532's graph to its n-1 adjacent-pair
// edges and both strategies are PathExistenceQueriesInAGraphISolution's - this
// file just pins them to LeetCode's published examples.
public sealed class PathExistenceQueriesInAGraphITests
{
    public static TheoryData<ProximityExample> Examples =>
        new()
        {
            {
                new ProximityExample(
                    N: 2,
                    Nums: [1, 3],
                    MaxDiff: 1,
                    Queries: [[0, 0], [0, 1]],
                    Expected: [true, false])
            },
            {
                new ProximityExample(
                    N: 4,
                    Nums: [2, 5, 6, 8],
                    MaxDiff: 2,
                    Queries: [[0, 1], [0, 2], [1, 3], [2, 3]],
                    Expected: [false, false, true, true])
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PathExistenceQueriesByBruteForceBfs_LeetCodeExamples_ReturnsWhetherEachQueryPairIsConnected(
        ProximityExample example)
    {
        var actual = PathExistenceQueriesInAGraphISolution.PathExistenceQueriesByBruteForceBfs(
            example.N, example.Nums, example.MaxDiff, example.Queries);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PathExistenceQueriesByDisjointSet_LeetCodeExamples_ReturnsWhetherEachQueryPairIsConnected(
        ProximityExample example)
    {
        var actual = PathExistenceQueriesInAGraphISolution.PathExistenceQueriesByDisjointSet(
            example.N, example.Nums, example.MaxDiff, example.Queries);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the vertex count, the values each vertex carries, the
    // largest gap still counted as adjacent, the query pairs, and each pair's answer.
    // The count and the gap are both `int`, so the fields name each one rather than
    // leaving two adjacent positions a caller could swap.
    public readonly record struct ProximityExample(
        int N,
        int[] Nums,
        int MaxDiff,
        int[][] Queries,
        bool[] Expected);
}
