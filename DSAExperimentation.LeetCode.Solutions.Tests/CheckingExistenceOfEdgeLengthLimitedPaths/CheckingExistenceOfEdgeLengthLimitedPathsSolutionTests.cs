using DSAExperimentation.LeetCode.Solutions.CheckingExistenceOfEdgeLengthLimitedPaths;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckingExistenceOfEdgeLengthLimitedPaths;

// Harness only: both strategies are
// CheckingExistenceOfEdgeLengthLimitedPathsSolution's. This file pins them to
// the boundaries LeetCode never published -
// a query whose endpoints are the same node, a limit exactly equal to the only
// edge's weight (the bound is strict, so it must not help), a parallel edge whose
// lighter copy is the one that counts, a pair in two different components, and a
// query that needs more than one hop.
public sealed partial class CheckingExistenceOfEdgeLengthLimitedPathsSolutionTests
{
    public static TheoryData<int, int[][], int[][], bool[]> Cases =>
        new()
        {
            // A node always reaches itself, with no edge crossed and no limit met.
            { 2, [[0, 1, 5]], [[0, 0, 1]], [true] },

            // The limit is strict: an edge of exactly that weight may not be used.
            { 2, [[0, 1, 5]], [[0, 1, 5], [0, 1, 6]], [false, true] },

            // Parallel edges of different weights - only the lighter copy counts.
            { 2, [[0, 1, 10], [0, 1, 3]], [[0, 1, 5]], [true] },

            // Two components: no limit joins them, however generous.
            {
                4,
                [[0, 1, 1], [2, 3, 1]],
                [[0, 3, 100], [0, 1, 2], [2, 3, 2]],
                [false, true, true]
            },

            // Multi-hop: every edge on the path has to clear the limit on its own.
            {
                4,
                [[0, 1, 1], [1, 2, 1], [2, 3, 1]],
                [[0, 3, 2], [0, 3, 1]],
                [true, false]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void DistanceLimitedPathsExistByPerQueryDfs_Cases_AnswersEachQueryInInputOrder(
        int nodeCount, int[][] edgeList, int[][] queries, bool[] expected)
    {
        var answers =
            CheckingExistenceOfEdgeLengthLimitedPathsSolution.DistanceLimitedPathsExistByPerQueryDfs(
                nodeCount, edgeList, queries);

        Assert.Equal(expected, answers);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void DistanceLimitedPathsExistByOfflineDisjointSet_Cases_AnswersEachQueryInInputOrder(
        int nodeCount, int[][] edgeList, int[][] queries, bool[] expected)
    {
        var answers =
            CheckingExistenceOfEdgeLengthLimitedPathsSolution.DistanceLimitedPathsExistByOfflineDisjointSet(
                nodeCount, edgeList, queries);

        Assert.Equal(expected, answers);
    }
}
