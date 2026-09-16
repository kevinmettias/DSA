using DSAExperimentation.LeetCode.MinimumNumberOfDaysToEatNOranges;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfDaysToEatNOranges;

// Harness only. Both strategies are MinimumNumberOfDaysToEatNOrangesSolution's -
// LeetCode's published examples plus the small values where the two branches of the
// recurrence tie (2, 3, 9) are asserted against each, so the unmemoized baseline is
// now held to the same answers as the memoized recurrence.
public sealed class MinimumNumberOfDaysToEatNOrangesTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 10, 4 },
            { 6, 3 },
            { 1, 1 },
            { 2, 2 },
            { 3, 2 },
            { 9, 3 },
            { 56, 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDaysByUnmemoizedRecursion_LeetCodeExamples_ReturnsMinimumDayCount(int n, int expected)
        => Assert.Equal(expected, MinimumNumberOfDaysToEatNOrangesSolution.MinDaysByUnmemoizedRecursion(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDaysByMemoizedRecurrence_LeetCodeExamples_ReturnsMinimumDayCount(int n, int expected)
        => Assert.Equal(expected, MinimumNumberOfDaysToEatNOrangesSolution.MinDaysByMemoizedRecurrence(n));
}
