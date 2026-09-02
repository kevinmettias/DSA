using DSAExperimentation.LeetCode.BlockPlacementQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BlockPlacementQueries;

// Harness only. Both strategies are BlockPlacementQueriesSolution's - this
// file just pins them to LeetCode's published examples, including the
// unopenable [2,2,2] case that depends on obstacle 0 always being present.
public sealed class BlockPlacementQueriesTests
{
    public static TheoryData<int[][], bool[]> Examples =>
        new()
        {
            {
                [[1, 2], [2, 3, 3], [2, 3, 1], [2, 2, 2]],
                [false, true, true]
            },
            {
                [[1, 7], [2, 7, 6], [1, 2], [2, 7, 5], [2, 7, 6]],
                [true, true, false]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPlaceByLinearScan_LeetCodeExamples_ReturnsPlacementResults(int[][] queries, bool[] expected) =>
        Assert.Equal(expected, BlockPlacementQueriesSolution.CanPlaceByLinearScan(queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPlaceBySegmentTreeMerge_LeetCodeExamples_ReturnsPlacementResults(int[][] queries, bool[] expected) =>
        Assert.Equal(expected, BlockPlacementQueriesSolution.CanPlaceBySegmentTreeMerge(queries));
}
