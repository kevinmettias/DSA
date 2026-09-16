using DSAExperimentation.LeetCode.ExamRoom;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ExamRoom;

// Harness only. Both strategies are ExamRoomSolution's - this file just replays
// LeetCode's published call scripts against each and asserts every Seat() result, so
// a failure names the strategy that broke. A script entry is either (Seat, 0) or
// (Leave, seatNumber); Seat() results are asserted in order against Expected.
public sealed partial class ExamRoomTests
{
    private const string SeatOperation = "seat";
    private const string LeaveOperation = "leave";

    public static TheoryData<int, (string Operation, int Argument)[], int[]> Examples =>
        new()
        {
            // LeetCode's published example: n = 10, seat/seat/seat/seat/leave(4)/seat.
            {
                10,
                [
                    (SeatOperation, 0), (SeatOperation, 0), (SeatOperation, 0),
                    (SeatOperation, 0), (LeaveOperation, 4), (SeatOperation, 0),
                ],
                [0, 9, 4, 2, 5]
            },

            // An empty room always seats at 0.
            { 5, [(SeatOperation, 0)], [0] },

            // A room filled to capacity, then reopened in the middle.
            {
                4,
                [
                    (SeatOperation, 0), (SeatOperation, 0), (SeatOperation, 0),
                    (SeatOperation, 0), (LeaveOperation, 1), (SeatOperation, 0),
                ],
                [0, 3, 1, 2, 1]
            },

            // Leaving the far end hands that end's whole run back to the next student.
            {
                10,
                [
                    (SeatOperation, 0), (SeatOperation, 0), (LeaveOperation, 9),
                    (SeatOperation, 0),
                ],
                [0, 9, 9]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLinearScanList_LeetCodeExamples_SeatsFarthestFromNearestStudent(
        int seatCount, (string Operation, int Argument)[] script, int[] expected) =>
        AssertScript(ExamRoomSolution.CreateByLinearScanList(seatCount), script, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBinarySearchDynamicArray_LeetCodeExamples_SeatsFarthestFromNearestStudent(
        int seatCount, (string Operation, int Argument)[] script, int[] expected) =>
        AssertScript(ExamRoomSolution.CreateByBinarySearchDynamicArray(seatCount), script, expected);

    private static void AssertScript(
        ExamRoomSolution.IExamRoom room, (string Operation, int Argument)[] script, int[] expected)
    {
        var seatIndex = 0;

        foreach (var (operation, argument) in script)
        {
            if (operation == LeaveOperation)
            {
                room.Leave(argument);
                continue;
            }

            Assert.Equal(expected[seatIndex], room.Seat());
            seatIndex++;
        }

        Assert.Equal(expected.Length, seatIndex);
    }
}
