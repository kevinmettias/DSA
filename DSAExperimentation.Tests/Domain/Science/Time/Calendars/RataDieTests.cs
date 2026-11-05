using DSAExperimentation.Domain.Science.Time.Calendars;

namespace DSAExperimentation.Tests.Domain.Science.Time.Calendars;

// Day 1's Monday is the fact; the tests read it against .NET's own count, which starts on the
// same day, and walk the week round both sides of it.
public sealed partial class RataDieTests
{
    public static TheoryData<long, DayOfWeek> DaysAndWeekdays => new()
    {
        { 1, DayOfWeek.Monday },
        { 2, DayOfWeek.Tuesday },
        { 6, DayOfWeek.Saturday },
        { 7, DayOfWeek.Sunday },
        { 8, DayOfWeek.Monday },
        { 0, DayOfWeek.Sunday },
        { -1, DayOfWeek.Saturday },
        { -6, DayOfWeek.Monday },
        { -7, DayOfWeek.Sunday },
    };

    // .NET's DateOnly starts on the same day, the Gregorian 1 January of year 1.
    [Fact]
    public void EpochWeekday_IsThePlatformsFirstDaysWeekday() =>
        Assert.Equal(RataDie.EpochWeekday, DateOnly.MinValue.DayOfWeek);

    [Theory]
    [MemberData(nameof(DaysAndWeekdays))]
    public void WeekdayOf_Day_IsCountedOnFromTheEpochsMonday(long dayNumber, DayOfWeek expected) =>
        Assert.Equal(expected, RataDie.WeekdayOf(dayNumber));
}
