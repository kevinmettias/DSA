using DSAExperimentation.LeetCode.MyCalendarIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MyCalendarIII;

// Harness only. Both strategies are MyCalendarIIISolution's - this file just
// replays LeetCode's published Book() call scripts against each and asserts
// the max-overlap-so-far result returned after every single call.
public sealed partial class MyCalendarIIITests
{
    public static TheoryData<(int Start, int End)[], int[]> Examples =>
        new()
        {
            {
                [(10, 20), (50, 60), (10, 40), (5, 15), (5, 10), (25, 55)],
                [1, 1, 2, 3, 3, 3]
            },
            { [(0, 5), (10, 15), (20, 25)], [1, 1, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByHashMapMergeSortSweep_LeetCodeExamples_ReturnsMaxOverlapAfterEachBooking(
        (int Start, int End)[] events, int[] expected) =>
        AssertSequence(MyCalendarIIISolution.CreateByHashMapMergeSortSweep(), events, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBruteForceEventRescan_LeetCodeExamples_ReturnsMaxOverlapAfterEachBooking(
        (int Start, int End)[] events, int[] expected) =>
        AssertSequence(MyCalendarIIISolution.CreateByBruteForceEventRescan(), events, expected);

    private static void AssertSequence(
        MyCalendarIIISolution.ICalendar calendar, (int Start, int End)[] events, int[] expected)
    {
        for (var i = 0; i < events.Length; i++)
        {
            var (start, end) = events[i];
            var maxOverlap = calendar.Book(start, end);

            Assert.Equal(expected[i], maxOverlap);
        }
    }
}
