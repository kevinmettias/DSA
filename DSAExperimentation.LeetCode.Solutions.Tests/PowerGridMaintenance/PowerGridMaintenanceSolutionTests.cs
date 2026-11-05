using DSAExperimentation.LeetCode.Solutions.PowerGridMaintenance;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PowerGridMaintenance;

// Harness only. Both strategies are PowerGridMaintenanceSolution's - this file just
// pins them to LeetCode's published examples, including the disconnected-stations
// case where a maintenance check has no grid-mate left to fall back on at all.
public sealed partial class PowerGridMaintenanceSolutionTests
{
    // Two grids, {1, 2} and {3, 4}: a pair shares a grid exactly when a chain of
    // connections joins it.
    public static TheoryData<int, int, bool> TwoGridStationPairs =>
        new()
        {
            { 1, 2, true },
            { 3, 4, true },
            { 2, 3, false },
        };

    [Theory]
    [MemberData(nameof(TwoGridStationPairs))]
    public void ConnectedGridsOf_TwoSeparateGrids_ConnectsOnlyStationsInTheSameGrid(int first, int second, bool expected)
    {
        var grids = PowerGridMaintenanceSolution.ConnectedGridsOf(4, [[1, 2], [3, 4]]);

        Assert.Equal(expected, grids.IsConnected(first, second));
    }
}
