using DSAExperimentation.LeetCode.Solutions.MeetingRoomsIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MeetingRoomsIII;

// Harness only: both strategies are MeetingRoomsIIISolution's. This pins the shapes
// the simulation has to get right - meetings listed out of start order, a delayed
// meeting that keeps its original duration, and a tie on the busiest room resolving
// to the lowest room number. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MeetingRoomsIIISolutionTests
{
    public static TheoryData<int, int[][], int> Cases =>
        new()
        {
            // LeetCode's example 1 with its meetings listed out of start order: the
            // answer only comes out right if the simulation sorts before it books.
            { 2, [[3, 4], [0, 10], [2, 7], [1, 5]], 0 },
            // Every meeting fits in room 0, so the spare rooms stay untouched.
            { 4, [[0, 5], [10, 15], [20, 25]], 0 },
            // One room, so the two later meetings are delayed and each keeps its own
            // one-unit duration rather than inheriting the blocker's end time.
            { 1, [[0, 10], [1, 2], [2, 3]], 0 },
            // Three rooms, one meeting each: every room is tied, so the lowest wins.
            { 3, [[0, 10], [1, 11], [2, 12]], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MostBookedByLinearScanFreeAt_Cases_ReturnsBusiestRoom(
        int roomCount, int[][] meetings, int expected)
    {
        var actual = MeetingRoomsIIISolution.MostBookedByLinearScanFreeAt(roomCount, meetings);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MostBookedByTwoHeapPool_Cases_ReturnsBusiestRoom(
        int roomCount, int[][] meetings, int expected)
    {
        var actual = MeetingRoomsIIISolution.MostBookedByTwoHeapPool(roomCount, meetings);

        Assert.Equal(expected, actual);
    }
}
