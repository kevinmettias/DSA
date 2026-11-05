using System.Runtime.CompilerServices;

namespace DSAExperimentation.Domain.Science.Time.Calendars;

// What a calendar of the Julian family says about its dates, read from its ICalendar witness:
// which years are leap years, how long a year and a month are, which day of its year a date is,
// the date's day on the shared count (RataDie), so how many days lie between two dates, and its
// day of the week. A month and a year are a calendar's units, not fixed lengths as TimeUnit's
// are: a month has 28 to 31 days and a year 365 or 366, so a length is asked of a year and a
// month. Over a whole cycle of the leap rule the lengths repeat, so a cycle's days are a fixed
// count, 146,097 in the Gregorian calendar's 400 years, derived here from the rule. The rule's
// cycles are static abstract members of a struct, so in a caller every division by one is by a
// constant and every count derived from them folds: DaysPerCycle compiles to 146,097. Years are
// counted from 1, the first year of the era, as both calendars number them.
/// <summary>A calendar's leap years, its years' and months' lengths, and its dates' days.</summary>
/// <typeparam name="TCalendar">The calendar, such as <see cref="GregorianCalendar"/>.</typeparam>
public static class Calendar<TCalendar>
    where TCalendar : struct, ICalendar
{
    /// <summary>The years after which the leap years repeat: the leap-century cycle.</summary>
    public static int YearsPerCycle => TCalendar.LeapCenturyCycle;

    /// <summary>The leap years in each cycle: 97 of the Gregorian calendar's 400.</summary>
    public static long LeapYearsPerCycle => LeapYearsThrough(YearsPerCycle);

    /// <summary>The days in each cycle: 146,097 in the Gregorian calendar's 400 years.</summary>
    public static long DaysPerCycle =>
        ((long)YearsPerCycle * CalendarMonths.DaysPerCommonYear) + LeapYearsPerCycle;

    /// <summary>Whether a year is a leap year, one with a 29 February.</summary>
    /// <param name="year">The year, from 1.</param>
    /// <returns>True when the leap-year cycle divides it and no century rule drops it.</returns>
    public static bool IsLeapYear(int year)
        => IsMultiple(year, TCalendar.LeapYearCycle) && !IsCommonCenturyYear(year);

    // A century year the leap-century cycle does not divide, which the rule makes a common year.
    // A calendar that keeps every century year has none, and says so with two equal constants, so
    // that test is made first: it folds per calendar, and the Julian calendar's leap test compiles
    // to one test of the year's low two bits. Without it the JIT kept a remainder by 100 compared
    // with itself. It asks to be inlined because DayNumber, which reaches it through DayOfYear,
    // had otherwise spent its inlining budget and called it.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsCommonCenturyYear(int year)
    {
        if (TCalendar.LeapCenturyCycle == TCalendar.CenturyCycle)
        {
            return false;
        }

        return IsMultiple(year, TCalendar.CenturyCycle)
            && !IsMultiple(year, TCalendar.LeapCenturyCycle);
    }

    /// <summary>Counts the days of a year.</summary>
    /// <param name="year">The year, from 1.</param>
    /// <returns>366 in a leap year, otherwise 365.</returns>
    public static int YearLength(int year)
    {
        var isLeapYear = IsLeapYear(year);

        return isLeapYear ? CalendarMonths.DaysPerLeapYear : CalendarMonths.DaysPerCommonYear;
    }

    /// <summary>Counts the days of a month in a given year.</summary>
    /// <param name="year">The year, from 1.</param>
    /// <param name="month">The month's number, January 1 to December 12.</param>
    /// <returns>The month's days, February's 29 in a leap year.</returns>
    public static int MonthLength(int year, int month)
    {
        var days = CalendarMonths.All[month - 1].DaysInCommonYear;
        if (month == CalendarMonths.LeapMonth && IsLeapYear(year))
        {
            days++;
        }

        return days;
    }

    /// <summary>Finds which day of its year a date is.</summary>
    /// <param name="date">The date, in this calendar.</param>
    /// <returns>The day of the year, 1 January's 1 to 31 December's 365 or 366.</returns>
    public static int DayOfYear(CalendarDate date)
    {
        var dayOfYear = CalendarMonths.DaysBeforeMonth[date.Month - 1] + date.Day;
        if (date.Month > CalendarMonths.LeapMonth && IsLeapYear(date.Year))
        {
            dayOfYear++;
        }

        return dayOfYear;
    }

    /// <summary>Finds a date's day on the count every calendar shares.</summary>
    /// <param name="date">The date, in this calendar.</param>
    /// <returns>The date's day number on <see cref="RataDie"/>'s count.</returns>
    public static long DayNumber(CalendarDate date)
        => TCalendar.Epoch + DaysBeforeYear(date.Year) + DayOfYear(date) - 1;

    // The days of the years before this one, back to year 1.
    private static long DaysBeforeYear(int year)
    {
        var yearsBefore = year - 1L;

        return (yearsBefore * CalendarMonths.DaysPerCommonYear) + LeapYearsThrough(yearsBefore);
    }

    // How many leap years the first so many years hold: those the leap-year cycle divides, less
    // the century years, and the leap-century years back again.
    private static long LeapYearsThrough(long years)
        => (years / TCalendar.LeapYearCycle) - (years / TCalendar.CenturyCycle)
            + (years / TCalendar.LeapCenturyCycle);

    /// <summary>Counts the days from one date to another.</summary>
    /// <param name="from">The date counted from, in this calendar.</param>
    /// <param name="to">The date counted to, in this calendar.</param>
    /// <returns>The days between them, negative when <paramref name="to"/> is earlier.</returns>
    public static long DaysBetween(CalendarDate from, CalendarDate to)
        => DayNumber(to) - DayNumber(from);

    /// <summary>Finds the day of the week a date falls on.</summary>
    /// <param name="date">The date, in this calendar.</param>
    /// <returns>The day of the week, read from the date's day on the shared count.</returns>
    public static DayOfWeek WeekdayOf(CalendarDate date)
    {
        var dayNumber = DayNumber(date);

        return RataDie.WeekdayOf(dayNumber);
    }

    private static bool IsMultiple(int year, int cycle) => year % cycle == 0;
}
