using DSAExperimentation.Domain.Science.Time.Calendars;

namespace DSAExperimentation.Tests.Domain.Science.Time.Calendars;

// The months are the fact itself, so the tests state every one with its number and its days, and
// read the year's lengths and the running totals back from them.
public sealed partial class CalendarMonthsTests
{
    public static TheoryData<CalendarMonth, int, int> NamedMonths => new()
    {
        { CalendarMonths.January, 1, 31 },
        { CalendarMonths.February, 2, 28 },
        { CalendarMonths.March, 3, 31 },
        { CalendarMonths.April, 4, 30 },
        { CalendarMonths.May, 5, 31 },
        { CalendarMonths.June, 6, 30 },
        { CalendarMonths.July, 7, 31 },
        { CalendarMonths.August, 8, 31 },
        { CalendarMonths.September, 9, 30 },
        { CalendarMonths.October, 10, 31 },
        { CalendarMonths.November, 11, 30 },
        { CalendarMonths.December, 12, 31 },
    };

    [Theory]
    [MemberData(nameof(NamedMonths))]
    public void NamedMonth_IsItsNumberAndDays(CalendarMonth month, int number, int days) =>
        Assert.Equal(new CalendarMonth(number, days), month);

    [Fact]
    public void All_IsTheTwelveMonthsEachAtItsNumber() =>
        Assert.Equal(Enumerable.Range(1, 12), CalendarMonths.All.Select(month => month.Number));

    [Fact]
    public void DaysPerCommonYear_IsTheMonthsDaysTogether()
    {
        Assert.Equal(365, CalendarMonths.DaysPerCommonYear);
        Assert.Equal(
            CalendarMonths.DaysPerCommonYear,
            CalendarMonths.All.Sum(month => month.DaysInCommonYear));
    }

    [Fact]
    public void DaysPerLeapYear_IsACommonYearAndTheLeapDay() =>
        Assert.Equal(366, CalendarMonths.DaysPerLeapYear);

    [Fact]
    public void LeapMonth_IsFebruary() =>
        Assert.Equal(CalendarMonths.LeapMonth, CalendarMonths.February.Number);

    [Fact]
    public void DaysBeforeMonth_IsEachMonthsStartInACommonYear() =>
        Assert.Equal<int>(
            [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334],
            CalendarMonths.DaysBeforeMonth);
}
