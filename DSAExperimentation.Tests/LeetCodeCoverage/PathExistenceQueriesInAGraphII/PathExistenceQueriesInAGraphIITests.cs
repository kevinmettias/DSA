using DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathExistenceQueriesInAGraphII;

// Harness only: both strategies live in PathExistenceQueriesInAGraphIISolution and
// are asserted against the same examples, so a failure names the strategy that
// broke. Example 2's [2,3] query and example 3's last two queries are the
// unreachable (-1) cases; example 2's [0,2] is the only query in this set that
// needs more than a single hop.
public sealed partial class PathExistenceQueriesInAGraphIITests
{
    public static TheoryData<PathExistenceCase> Examples =>
        new()
        {
            { new PathExistenceCase(N: 5, Nums: [1, 8, 3, 4, 2], MaxDiff: 3, Queries: [[0, 3], [2, 4]], Expected: [1, 1]) },
            { new PathExistenceCase(N: 5, Nums: [5, 3, 1, 9, 10], MaxDiff: 2, Queries: [[0, 1], [0, 2], [2, 3], [4, 3]], Expected: [1, 2, -1, 1]) },
            { new PathExistenceCase(N: 3, Nums: [3, 6, 1], MaxDiff: 1, Queries: [[0, 0], [0, 1], [1, 2]], Expected: [0, -1, -1]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistancesByRangeBfs_LeetCodeExamples_ReturnsShortestHopCountPerQuery(
        PathExistenceCase example)
    {
        var actual = PathExistenceQueriesInAGraphIISolution.MinDistancesByRangeBfs(
            example.N, example.Nums, example.MaxDiff, example.Queries);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistancesByBinaryLifting_LeetCodeExamples_ReturnsShortestHopCountPerQuery(
        PathExistenceCase example)
    {
        var actual = PathExistenceQueriesInAGraphIISolution.MinDistancesByBinaryLifting(
            example.N, example.Nums, example.MaxDiff, example.Queries);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the node count, the value array that decides which nodes
    // can be a hop apart, the largest value difference that still counts as an edge,
    // the queries as [from, to] pairs, and the shortest hop count per query (-1 where
    // unreachable). Two of the fields are jagged arrays of int, so each is named at
    // every construction site and a row reads as the case it is rather than as two
    // positions a caller has to keep in order. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct PathExistenceCase(int N, int[] Nums, int MaxDiff, int[][] Queries, int[] Expected);
}
