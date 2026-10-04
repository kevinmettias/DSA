using DSAExperimentation.Algorithms.ShortestPaths.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Grids;

public sealed partial class ParityBounceArrivalTests
{
    [Fact]
    public void Arrive_CellAlreadyOpen_TakesOneSecond() => Assert.Equal(6, ParityBounceArrival.Arrive(5, 3));

    // Landing at second 1 is four seconds early; two round trips cover it exactly.
    [Fact]
    public void Arrive_EvenShortfall_LandsWhenTheCellOpens() => Assert.Equal(5, ParityBounceArrival.Arrive(0, 5));

    // Landing at second 2 is three seconds early; bouncing only ever covers an even number, so the
    // walker lands one second after the cell opens.
    [Fact]
    public void Arrive_OddShortfall_LandsOneSecondLater() => Assert.Equal(6, ParityBounceArrival.Arrive(1, 5));
}
