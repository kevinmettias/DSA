using DSAExperimentation.LeetCode.MyCalendarI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MyCalendarI;

// Harness only. Both strategies are MyCalendarISolution's - this file just
// replays LeetCode's published Book() call scripts against each and asserts
// the accept/reject result of every single call, including the touching-
// endpoints case neither strategy may treat as a conflict.
public sealed class MyCalendarITests
{
    public static TheoryData<(int Start, int End)[], bool[]> Examples =>
        new()
        {
            { [(10, 20), (15, 25), (20, 30)], [true, false, true] },
            { [(5, 10), (10, 15)], [true, true] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByIntervalSet_LeetCodeExamples_RejectsOnlyOverlappingEvents(
        (int Start, int End)[] events, bool[] expected) =>
        AssertSequence(MyCalendarISolution.CreateByIntervalSet(), events, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLinearScan_LeetCodeExamples_RejectsOnlyOverlappingEvents(
        (int Start, int End)[] events, bool[] expected) =>
        AssertSequence(MyCalendarISolution.CreateByLinearScan(), events, expected);

    private static void AssertSequence(
        MyCalendarISolution.ICalendar calendar, (int Start, int End)[] events, bool[] expected)
    {
        for (var i = 0; i < events.Length; i++)
        {
            var (start, end) = events[i];
            Assert.Equal(expected[i], calendar.Book(start, end));
        }
    }
}
