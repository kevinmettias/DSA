using DSAExperimentation.LeetCode.ClosestRoom;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestRoom;

// Harness only. Both the per-query scan baseline and the sorted sweep over an
// always-sorted DynamicArray<int> are ClosestRoomSolution's - this file just pins
// them to LeetCode's published examples plus the three tie and boundary cases the
// insertion-point probe has to get right: a distance tie whose smaller id is found
// last (so the baseline's own tie-break is exercised, not just the sweep's), a
// preferred id below every eligible one (no floor), and one above them all (no
// ceiling). The scan arm used to exist only as an unasserted benchmark baseline.
public sealed class ClosestRoomTests
{
    public static TheoryData<int[][], int[][], int[]> Examples =>
        new()
        {
            { [[2, 2], [1, 2], [3, 2]], [[3, 1], [3, 3], [5, 2]], [3, -1, 3] },
            { [[1, 4], [2, 3], [3, 5], [4, 1], [5, 2]], [[2, 3], [2, 4], [2, 5]], [2, 1, 3] },
            { [[5, 2], [3, 2]], [[4, 1]], [3] },
            { [[10, 5], [20, 5]], [[1, 5], [30, 5], [15, 5]], [10, 20, 10] },
            { [[1, 1]], [[1, 1], [1, 2]], [1, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindClosestRoomsByPerQueryScan_LeetCodeExamples_ReturnsClosestBigEnoughRoomIds(
        int[][] rooms, int[][] queries, int[] expected) =>
        Assert.Equal(expected, ClosestRoomSolution.FindClosestRoomsByPerQueryScan(rooms, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindClosestRoomsBySortedSweep_LeetCodeExamples_ReturnsClosestBigEnoughRoomIds(
        int[][] rooms, int[][] queries, int[] expected) =>
        Assert.Equal(expected, ClosestRoomSolution.FindClosestRoomsBySortedSweep(rooms, queries));
}
