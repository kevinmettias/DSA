using DSAExperimentation.LeetCode.PowerGridMaintenance;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerGridMaintenance;

// Harness only. Both strategies are PowerGridMaintenanceSolution's - this file just
// pins them to LeetCode's published examples, including the disconnected-stations
// case where a maintenance check has no grid-mate left to fall back on at all.
public sealed partial class PowerGridMaintenanceTests
{
    public static TheoryData<int, int[][], int[][], int[]> Examples =>
        new()
        {
            {
                5,
                [[1, 2], [2, 3], [3, 4], [4, 5]],
                [[1, 3], [2, 1], [1, 1], [2, 2], [1, 2]],
                [3, 2, 3]
            },
            {
                3,
                [],
                [[1, 1], [2, 1], [1, 1]],
                [1, -1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaintenanceResultsByUnionFindSortedSet_LeetCodeExamples_ReturnsSmallestOnlineStationPerCheck(
        int stationCount, int[][] connections, int[][] queries, int[] expected)
    {
        var actual = PowerGridMaintenanceSolution.MaintenanceResultsByUnionFindSortedSet(stationCount, connections, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaintenanceResultsByUnionFindHeap_LeetCodeExamples_ReturnsSmallestOnlineStationPerCheck(
        int stationCount, int[][] connections, int[][] queries, int[] expected)
    {
        var actual = PowerGridMaintenanceSolution.MaintenanceResultsByUnionFindHeap(stationCount, connections, queries);

        Assert.Equal(expected, actual);
    }
}
