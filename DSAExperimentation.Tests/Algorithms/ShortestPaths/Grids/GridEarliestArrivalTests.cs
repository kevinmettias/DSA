using DSAExperimentation.Algorithms.ShortestPaths.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Grids;

public sealed partial class GridEarliestArrivalTests
{
    // LC 3341's examples: a cell's time is when a move into it may begin.
    public static TheoryData<int[][], int> WaitInPlaceExamples =>
        new()
        {
            { [[0, 4], [4, 4]], 6 },
            { [[0, 0, 0], [0, 0, 0]], 3 },
            { [[0, 1], [1, 2]], 3 },
        };

    [Theory]
    [MemberData(nameof(WaitInPlaceExamples))]
    public void Time_WaitInPlace_ReachesTheFarCornerAtTheEarliestSecond(int[][] cellTimes, int expected) =>
        Assert.Equal(expected, GridEarliestArrival.Time<WaitInPlaceArrival>(cellTimes, (0, 0), FarCorner(cellTimes)));

    // LC 2577's first example: a cell's time is when it may be entered, and an early walker bounces.
    [Fact]
    public void Time_ParityBounce_ReachesTheFarCornerAtTheEarliestSecond()
    {
        int[][] cellTimes = [[0, 1, 3, 2], [5, 1, 2, 5], [4, 3, 8, 6]];

        Assert.Equal(7, GridEarliestArrival.Time<ParityBounceArrival>(cellTimes, (0, 0), FarCorner(cellTimes)));
    }

    // The straight route crosses a cell that opens at second 100; the four-move detour around it wins.
    [Fact]
    public void Time_SlowCellOnTheStraightRoute_DetoursAroundIt()
    {
        int[][] cellTimes = [[0, 0, 0], [0, 100, 0], [0, 0, 0]];

        Assert.Equal(4, GridEarliestArrival.Time<WaitInPlaceArrival>(cellTimes, (1, 0), (1, 2)));
    }

    // The start is not (0, 0), so the search is not leaning on starting at the origin: on a grid that is
    // open from the outset, each move costs one second and the answer is the Manhattan distance.
    [Fact]
    public void Time_InteriorStartOnAnOpenGrid_IsTheManhattanDistance() =>
        Assert.Equal(2, GridEarliestArrival.Time<WaitInPlaceArrival>([[0, 0, 0], [0, 0, 0], [0, 0, 0]], (1, 1), (0, 2)));

    [Fact]
    public void Time_StartIsTheTarget_IsZero() =>
        Assert.Equal(0, GridEarliestArrival.Time<ParityBounceArrival>([[0, 9], [9, 9]], (1, 1), (1, 1)));

    [Fact]
    public void Time_TargetOffTheGrid_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => GridEarliestArrival.Time<WaitInPlaceArrival>([[0, 0]], (0, 0), (1, 0)));

    private static (int Row, int Col) FarCorner(int[][] cellTimes) => (cellTimes.Length - 1, cellTimes[0].Length - 1);
}
