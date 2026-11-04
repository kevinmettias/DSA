using DSAExperimentation.LeetCode.Solutions.MeetingRoomsIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MeetingRoomsIII;

public sealed partial class MeetingRoomsIIIPromiseTests
{
    public static TheoryData<int, int[][], bool> Inputs =>
        new()
        {
            { 2, [[0, 10], [1, 5], [2, 7], [3, 4]], true },
            { 2, [[0, 10], [0, 5]], false },
            { 2, [[5, 5]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenStartsAreUniqueAndBeforeEnds(int roomCount, int[][] meetings, bool expected)
        => Assert.Equal(expected, MeetingRoomsIIIPromise.IsKeptBy(roomCount, meetings));
}
