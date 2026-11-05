using DSAExperimentation.LeetCode.Solutions.ClosestRoom;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ClosestRoom;

// Harness only. Both the per-query scan baseline and the sorted sweep over an
// always-sorted DynamicArray<int> are ClosestRoomSolution's - this file just pins
// them to the three tie and boundary cases the
// insertion-point probe has to get right: a distance tie whose smaller id is found
// last (so the baseline's own tie-break is exercised, not just the sweep's), a
// preferred id below every eligible one (no floor), and one above them all (no
// ceiling). The scan arm used to exist only as an unasserted benchmark baseline.
public sealed partial class ClosestRoomSolutionTests
{
    public static TheoryData<int[][], int[][], int[]> Cases =>
        new()
        {
            { [[5, 2], [3, 2]], [[4, 1]], [3] },
            { [[10, 5], [20, 5]], [[1, 5], [30, 5], [15, 5]], [10, 20, 10] },
            { [[1, 1]], [[1, 1], [1, 2]], [1, -1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindClosestRoomsByPerQueryScan_Cases_ReturnsClosestBigEnoughRoomIds(
        int[][] rooms, int[][] queries, int[] expected)
    {
        var actual = ClosestRoomSolution.FindClosestRoomsByPerQueryScan(rooms, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindClosestRoomsBySortedSweep_Cases_ReturnsClosestBigEnoughRoomIds(
        int[][] rooms, int[][] queries, int[] expected)
    {
        var actual = ClosestRoomSolution.FindClosestRoomsBySortedSweep(rooms, queries);

        Assert.Equal(expected, actual);
    }
}
