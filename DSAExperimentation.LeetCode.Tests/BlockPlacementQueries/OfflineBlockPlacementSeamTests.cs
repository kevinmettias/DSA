using DSAExperimentation.LeetCode.BlockPlacementQueries;

namespace DSAExperimentation.LeetCode.Tests.BlockPlacementQueries;

// The seam between BlockPlacementQueriesSolution's two query processors.
// CanPlaceByLinearScan keeps obstacles in a sorted BCL List and rescans it per
// query. CanPlaceBySegmentTreeMerge answers the same queries by walking them in
// REVERSE, so every "place an obstacle" becomes a deactivation, and composes three
// DataStructures primitives to do it: a point-update SegmentTree<long,
// MaxOperation<long>> holding each gap indexed by its right-hand obstacle, plus
// NearestActiveObstacle, which drives two DisjointSetForest instances through its
// own path-compressing Find.
//
// The reverse walk is what makes this a seam rather than a reimplementation: the
// answer for query t is read at a point in time when the segment tree and both
// forests describe the obstacle set as of t, and any drift between the tree's gaps
// and the forests' nearest-active answers lands directly in the returned bool. The
// forward arm is the only reference that can see that drift.
public sealed partial class OfflineBlockPlacementSeamTests
{
    [Fact]
    public void CanPlace_LeetCodeExample_MatchesLinearScan()
    {
        int[][] queries = [[1, 2], [2, 3, 3], [2, 3, 1], [2, 2, 2]];

        Assert.Equal(new[] { false, true, true }, BlockPlacementQueriesSolution.CanPlaceByLinearScan(queries));
        AssertSameAnswers(queries);
    }

    // The smallest coordinate the problem allows, where the permanent obstacle at 0
    // is the only one: an empty gap tree has to answer a range query over an index
    // that holds MaxOperation's identity and nothing else.
    [Fact]
    public void CanPlace_AtTheSmallestCoordinate_MatchesLinearScan()
    {
        int[][] queries = [[2, 1, 1], [2, 1, 2], [1, 1], [2, 1, 1]];

        AssertSameAnswers(queries);
    }

    [Fact]
    public void CanPlace_BeforeAnyObstacleIsPlaced_MatchesLinearScan()
    {
        int[][] queries = [[2, 5, 5], [2, 5, 6], [1, 3], [2, 5, 3]];

        AssertSameAnswers(queries);
    }

    // A coordinate placed and then asked about directly: the reverse walk restores
    // the obstacle before the query that wanted it is answered, so the forests and
    // the gap tree must agree about a gap that only exists at one instant.
    [Fact]
    public void PlaceThenQueryThePlacedCoordinate_MatchesLinearScan()
    {
        int[][] queries = [[1, 4], [2, 9, 4], [2, 9, 6], [2, 4, 5], [2, 4, 4]];

        AssertSameAnswers(queries);
    }

    // Obstacles placed out of coordinate order: the reverse walk restores them from
    // the highest hit backwards, so each restore rewrites a gap between two
    // neighbours the forests must have already reconnected.
    [Fact]
    public void Place_OutOfCoordinateOrder_MatchesLinearScan()
    {
        int[][] queries =
        [
            [1, 8],
            [1, 3],
            [1, 6],
            [2, 9, 5],
            [2, 7, 3],
            [1, 2],
            [2, 9, 2],
        ];

        AssertSameAnswers(queries);
    }

    [Fact]
    public void CanPlace_BlockExactlyFillingTheLargestGap_MatchesLinearScan()
    {
        int[][] queries = [[1, 5], [2, 5, 5], [2, 5, 6], [2, 4, 4], [2, 4, 5]];

        AssertSameAnswers(queries);
    }

    [Fact]
    public void CanPlace_NoTypeTwoQueryAtAll_AnswersEmptyOnBothArms()
    {
        int[][] queries = [[1, 1], [1, 2], [1, 3]];

        Assert.Empty(BlockPlacementQueriesSolution.CanPlaceBySegmentTreeMerge(queries));
        AssertSameAnswers(queries);
    }

    private static void AssertSameAnswers(int[][] queries)
        => Assert.Equal(
            BlockPlacementQueriesSolution.CanPlaceByLinearScan(queries),
            BlockPlacementQueriesSolution.CanPlaceBySegmentTreeMerge(queries));
}
