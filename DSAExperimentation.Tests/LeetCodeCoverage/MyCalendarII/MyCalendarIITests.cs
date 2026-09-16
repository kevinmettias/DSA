using DSAExperimentation.LeetCode.MyCalendarII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MyCalendarII;

// Harness only. Both strategies are MyCalendarIISolution's - this file just
// replays LeetCode's published Book() call scripts against each and asserts
// the accept/reject result of every single call, including the touching-
// endpoints case neither strategy may treat as a double booking.
public sealed partial class MyCalendarIITests
{
    public static TheoryData<(int Start, int End)[], bool[]> Examples =>
        new()
        {
            {
                [(10, 20), (50, 60), (10, 40), (5, 15), (5, 10), (25, 55)],
                [true, true, true, false, true, true]
            },
            { [(5, 10), (10, 15), (0, 5)], [true, true, true] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByTwoIntervalSetScan_LeetCodeExamples_RejectsOnlyTripleBookings(
        (int Start, int End)[] events, bool[] expected) =>
        AssertSequence(MyCalendarIISolution.CreateByTwoIntervalSetScan(), events, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByTwoListScan_LeetCodeExamples_RejectsOnlyTripleBookings(
        (int Start, int End)[] events, bool[] expected) =>
        AssertSequence(MyCalendarIISolution.CreateByTwoListScan(), events, expected);

    private static void AssertSequence(
        MyCalendarIISolution.ICalendar calendar, (int Start, int End)[] events, bool[] expected)
    {
        for (var i = 0; i < events.Length; i++)
        {
            var (start, end) = events[i];
            var accepted = calendar.Book(start, end);

            Assert.Equal(expected[i], accepted);
        }
    }
}
