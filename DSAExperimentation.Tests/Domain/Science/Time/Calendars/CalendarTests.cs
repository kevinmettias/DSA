using DSAExperimentation.Domain.Science.Time.Calendars;
using Gregorian = DSAExperimentation.Domain.Science.Time.Calendars.Calendar<
    DSAExperimentation.Domain.Science.Time.Calendars.GregorianCalendar>;
using Julian = DSAExperimentation.Domain.Science.Time.Calendars.Calendar<
    DSAExperimentation.Domain.Science.Time.Calendars.JulianCalendar>;
using PlatformJulianCalendar = System.Globalization.JulianCalendar;

namespace DSAExperimentation.Tests.Domain.Science.Time.Calendars;

// The rules are the facts, so the tests state what history fixes: which century years each
// calendar keeps as leap years, a cycle's days counted from the years the rule makes, the reform's
// Thursday 4 October 1582 followed by Friday 15 October, and LeetCode's examples (LC 1154, 1185
// and 1360). .NET counts days from the same first day, for the Gregorian calendar and for the
// Julian, so every day of a whole cycle of each is also read against the platform's reading of it.
public sealed partial class CalendarTests
{
    // .NET's reading of the Julian calendar, the oracle for every day of a Julian cycle.
    private static readonly PlatformJulianCalendar JulianPlatform = new();

    public static TheoryData<int> GregorianLeapYears => new() { 4, 1600, 2000, 2024, 2400 };

    public static TheoryData<int> GregorianCommonYears =>
        new() { 1, 100, 1700, 1800, 1900, 2023, 2100 };

    public static TheoryData<int> JulianLeapYears => new() { 4, 100, 1700, 1900, 2000, 2100 };

    public static TheoryData<int> JulianCommonYears => new() { 1, 1582, 2023 };

    public static TheoryData<int, int, int> GregorianMonthLengths => new()
    {
        { 2023, 1, 31 }, { 2023, 2, 28 }, { 2024, 2, 29 }, { 1900, 2, 28 }, { 2000, 2, 29 },
        { 2023, 4, 30 }, { 2024, 12, 31 },
    };

    public static TheoryData<int, int, int, int> GregorianDaysOfTheYear => new()
    {
        { 2019, 1, 9, 9 }, { 2019, 2, 10, 41 }, { 2023, 3, 1, 60 }, { 2024, 3, 1, 61 },
        { 1900, 3, 1, 60 }, { 2000, 3, 1, 61 }, { 2023, 12, 31, 365 }, { 2024, 12, 31, 366 },
    };

    public static TheoryData<int, int, int, long> GregorianDayNumbers => new()
    {
        { 1, 1, 1, 1 }, { 1582, 10, 15, 577_736 }, { 1970, 1, 1, 719_163 },
        { 2000, 1, 1, 730_120 },
    };

    public static TheoryData<int, int, int, DayOfWeek> GregorianWeekdays => new()
    {
        { 2019, 8, 31, DayOfWeek.Saturday }, { 1999, 7, 18, DayOfWeek.Sunday },
        { 1993, 8, 15, DayOfWeek.Sunday }, { 1970, 1, 1, DayOfWeek.Thursday },
        { 2000, 1, 1, DayOfWeek.Saturday }, { 1, 1, 1, DayOfWeek.Monday },
    };

    [Theory]
    [MemberData(nameof(GregorianLeapYears))]
    public void IsLeapYear_GregorianLeapYear_IsTrue(int year) =>
        Assert.True(Gregorian.IsLeapYear(year));

    [Theory]
    [MemberData(nameof(GregorianCommonYears))]
    public void IsLeapYear_GregorianCommonYearOrCenturyFourHundredDoesNotDivide_IsFalse(int year) =>
        Assert.False(Gregorian.IsLeapYear(year));

    [Theory]
    [MemberData(nameof(JulianLeapYears))]
    public void IsLeapYear_JulianEveryFourthYearCenturiesIncluded_IsTrue(int year) =>
        Assert.True(Julian.IsLeapYear(year));

    [Theory]
    [MemberData(nameof(JulianCommonYears))]
    public void IsLeapYear_JulianYearFourDoesNotDivide_IsFalse(int year) =>
        Assert.False(Julian.IsLeapYear(year));

    [Fact]
    public void YearsPerCycle_IsTheLeapCenturyCycle()
    {
        Assert.Equal(400, Gregorian.YearsPerCycle);
        Assert.Equal(100, Julian.YearsPerCycle);
    }

    [Fact]
    public void LeapYearsPerCycle_IsTheLeapYearsTheRuleMakesInACycle()
    {
        Assert.Equal(97, Gregorian.LeapYearsPerCycle);
        Assert.Equal(LeapYearsInFirstCycle<GregorianCalendar>(), Gregorian.LeapYearsPerCycle);
        Assert.Equal(25, Julian.LeapYearsPerCycle);
        Assert.Equal(LeapYearsInFirstCycle<JulianCalendar>(), Julian.LeapYearsPerCycle);
    }

    // The 146,097 days are derived in the calendar from its cycles; here they are counted year by
    // year from the leap-year rule, over the first cycle and over the one 1700, 1800 and 1900 fall
    // in.
    [Fact]
    public void DaysPerCycle_Gregorian_IsTheDaysOfTheYearsItHolds()
    {
        Assert.Equal(146_097, Gregorian.DaysPerCycle);
        Assert.Equal(DaysOfCycleFrom<GregorianCalendar>(1), Gregorian.DaysPerCycle);
        Assert.Equal(DaysOfCycleFrom<GregorianCalendar>(1601), Gregorian.DaysPerCycle);
    }

    [Fact]
    public void DaysPerCycle_Julian_IsTheDaysOfTheYearsItHolds()
    {
        Assert.Equal(36_525, Julian.DaysPerCycle);
        Assert.Equal(DaysOfCycleFrom<JulianCalendar>(1), Julian.DaysPerCycle);
        Assert.Equal(DaysOfCycleFrom<JulianCalendar>(1501), Julian.DaysPerCycle);
    }

    // So a Gregorian date falls on the same weekday every 400 years. A Julian century is not a
    // whole number of weeks; its weekdays repeat after 28 years, four leap cycles of seven.
    [Fact]
    public void DaysPerCycle_Gregorian_IsAWholeNumberOfWeeks()
    {
        Assert.Equal(0, Gregorian.DaysPerCycle % 7);
        Assert.Equal(
            Gregorian.WeekdayOf(new CalendarDate(2026, 11, 5)),
            Gregorian.WeekdayOf(new CalendarDate(2426, 11, 5)));
        Assert.NotEqual(0, Julian.DaysPerCycle % 7);
    }

    // The mean years are 365.2425 and 365.25 days, compared exactly in ten-thousandths of a day:
    // a cycle's days over its years, cross-multiplied.
    [Fact]
    public void DaysPerCycle_OverYearsPerCycle_IsTheMeanYear()
    {
        Assert.Equal(
            3_652_425L * Gregorian.YearsPerCycle,
            Gregorian.DaysPerCycle * 10_000);
        Assert.Equal(
            3_652_500L * Julian.YearsPerCycle,
            Julian.DaysPerCycle * 10_000);
    }

    [Fact]
    public void YearLength_LeapYear_HasTheLeapDay()
    {
        Assert.Equal(366, Gregorian.YearLength(2024));
        Assert.Equal(365, Gregorian.YearLength(2023));
        Assert.Equal(365, Gregorian.YearLength(1900));
        Assert.Equal(366, Julian.YearLength(1900));
    }

    [Theory]
    [MemberData(nameof(GregorianMonthLengths))]
    public void MonthLength_Gregorian_IsTheMonthsDaysThatYear(int year, int month, int expected) =>
        Assert.Equal(expected, Gregorian.MonthLength(year, month));

    [Fact]
    public void MonthLength_JulianFebruaryOfACenturyYear_HasTheLeapDay() =>
        Assert.Equal(29, Julian.MonthLength(1900, CalendarMonths.LeapMonth));

    [Fact]
    public void MonthLength_EveryMonthOfAYear_AddsUpToTheYear()
    {
        Assert.All(
            [1900, 2000, 2023, 2024],
            year => Assert.Equal(Gregorian.YearLength(year), MonthsTotal<GregorianCalendar>(year)));
        Assert.All(
            [1900, 2023],
            year => Assert.Equal(Julian.YearLength(year), MonthsTotal<JulianCalendar>(year)));
    }

    [Theory]
    [MemberData(nameof(GregorianDaysOfTheYear))]
    public void DayOfYear_Gregorian_CountsTheMonthsBeforeAndTheLeapDay(
        int year, int month, int day, int expected) =>
        Assert.Equal(expected, Gregorian.DayOfYear(new CalendarDate(year, month, day)));

    [Theory]
    [MemberData(nameof(GregorianDayNumbers))]
    public void DayNumber_Gregorian_IsTheDayOnTheSharedCount(
        int year, int month, int day, long expected) =>
        Assert.Equal(expected, Gregorian.DayNumber(new CalendarDate(year, month, day)));

    // The reform dropped ten days: Thursday 4 October 1582 in the Julian calendar was followed by
    // Friday 15 October in the Gregorian.
    [Fact]
    public void DayNumber_GregorianReform_FollowsTheLastJulianDay()
    {
        var lastJulianDay = new CalendarDate(1582, 10, 4);
        var firstGregorianDay = new CalendarDate(1582, 10, 15);

        Assert.Equal(
            Julian.DayNumber(lastJulianDay) + 1,
            Gregorian.DayNumber(firstGregorianDay));
        Assert.Equal(DayOfWeek.Thursday, Julian.WeekdayOf(lastJulianDay));
        Assert.Equal(DayOfWeek.Friday, Gregorian.WeekdayOf(firstGregorianDay));
    }

    [Fact]
    public void DayNumber_ThirdCentury_IsTheSameDayInBothCalendars() =>
        Assert.All(
            [new CalendarDate(200, 3, 1), new CalendarDate(250, 7, 1), new(300, 2, 28)],
            date => Assert.Equal(Julian.DayNumber(date), Gregorian.DayNumber(date)));

    [Fact]
    public void DayNumber_EveryGregorianDayOfACycle_IsThePlatformsReading()
    {
        var first = new DateOnly(1601, 1, 1);
        var days = Enumerable.Range(0, (int)Gregorian.DaysPerCycle)
            .Select(first.AddDays)
            .ToList();

        Assert.Equal(days.Select(PlatformGregorianReading), days.Select(GregorianReading));
    }

    [Fact]
    public void DayNumber_EveryJulianDayOfACycle_IsThePlatformsReading()
    {
        var first = JulianPlatform.ToDateTime(1501, 1, 1, 0, 0, 0, 0);
        var days = Enumerable.Range(0, (int)Julian.DaysPerCycle)
            .Select(offset => first.AddDays(offset))
            .ToList();

        Assert.Equal(days.Select(PlatformJulianReading), days.Select(JulianReading));
    }

    [Fact]
    public void DaysBetween_LeetCodeExamples_CountForwardAndBack()
    {
        Assert.Equal(1, Gregorian.DaysBetween(new(2019, 6, 29), new(2019, 6, 30)));
        Assert.Equal(-15, Gregorian.DaysBetween(new(2020, 1, 15), new(2019, 12, 31)));
    }

    [Fact]
    public void DaysBetween_ACycleApart_IsTheCyclesDays() =>
        Assert.Equal(
            Gregorian.DaysPerCycle,
            Gregorian.DaysBetween(new(1601, 3, 1), new(2001, 3, 1)));

    [Theory]
    [MemberData(nameof(GregorianWeekdays))]
    public void WeekdayOf_Gregorian_IsTheDatesWeekday(
        int year, int month, int day, DayOfWeek expected) =>
        Assert.Equal(expected, Gregorian.WeekdayOf(new CalendarDate(year, month, day)));

    private static long LeapYearsInFirstCycle<TCalendar>()
        where TCalendar : struct, ICalendar
        => Enumerable.Range(1, Calendar<TCalendar>.YearsPerCycle)
            .Count(Calendar<TCalendar>.IsLeapYear);

    private static long DaysOfCycleFrom<TCalendar>(int firstYear)
        where TCalendar : struct, ICalendar
        => Enumerable.Range(firstYear, Calendar<TCalendar>.YearsPerCycle)
            .Sum(Calendar<TCalendar>.YearLength);

    private static int MonthsTotal<TCalendar>(int year)
        where TCalendar : struct, ICalendar
        => CalendarMonths.All.Sum(month => Calendar<TCalendar>.MonthLength(year, month.Number));

    private static DayReading GregorianReading(DateOnly day)
        => ReadingOf<GregorianCalendar>(new CalendarDate(day.Year, day.Month, day.Day));

    // .NET's DayNumber counts from 0 on the Gregorian 1 January of year 1, the shared count's
    // day 1.
    private static DayReading PlatformGregorianReading(DateOnly day)
        => new(
            day.DayNumber + GregorianCalendar.Epoch,
            day.DayOfYear,
            day.DayOfWeek,
            DateTime.DaysInMonth(day.Year, day.Month),
            DateTime.IsLeapYear(day.Year));

    private static DayReading JulianReading(DateTime day)
    {
        var date = new CalendarDate(
            JulianPlatform.GetYear(day),
            JulianPlatform.GetMonth(day),
            JulianPlatform.GetDayOfMonth(day));

        return ReadingOf<JulianCalendar>(date);
    }

    private static DayReading PlatformJulianReading(DateTime day)
    {
        var year = JulianPlatform.GetYear(day);

        return new(
            DateOnly.FromDateTime(day).DayNumber + GregorianCalendar.Epoch,
            JulianPlatform.GetDayOfYear(day),
            day.DayOfWeek,
            JulianPlatform.GetDaysInMonth(year, JulianPlatform.GetMonth(day)),
            JulianPlatform.IsLeapYear(year));
    }

    private static DayReading ReadingOf<TCalendar>(CalendarDate date)
        where TCalendar : struct, ICalendar
        => new(
            Calendar<TCalendar>.DayNumber(date),
            Calendar<TCalendar>.DayOfYear(date),
            Calendar<TCalendar>.WeekdayOf(date),
            Calendar<TCalendar>.MonthLength(date.Year, date.Month),
            Calendar<TCalendar>.IsLeapYear(date.Year));

    // What a calendar and the platform each say about one day, compared whole.
    private readonly record struct DayReading(
        long DayNumber, int DayOfYear, DayOfWeek Weekday, int MonthLength, bool IsLeapYear);
}
