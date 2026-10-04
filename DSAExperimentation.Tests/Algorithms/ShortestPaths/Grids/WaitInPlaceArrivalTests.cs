using DSAExperimentation.Algorithms.ShortestPaths.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Grids;

public sealed partial class WaitInPlaceArrivalTests
{
    [Fact]
    public void Arrive_CellAlreadyOpen_TakesOneSecond() => Assert.Equal(6, WaitInPlaceArrival.Arrive(5, 3));

    // Waiting where you stand: the move begins at second 7 and lands at 8.
    [Fact]
    public void Arrive_CellNotYetOpen_WaitsForItThenTakesOneSecond() => Assert.Equal(8, WaitInPlaceArrival.Arrive(2, 7));
}
