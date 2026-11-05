namespace DSAExperimentation.Domain.Science.Time.Calendars;

// One count of days that every calendar's dates fall on, the rata die of Reingold and
// Dershowitz's Calendrical Calculations: day 1 is 1 January of year 1 in the Gregorian calendar
// run back before its reform, the Gregorian calendar's epoch. A date in any calendar is a day on
// it (Calendar.DayNumber), so two dates are a subtraction apart whichever calendars they are
// written in. The seven-day week runs through the count unbroken, so a day's weekday is read from
// a day whose weekday is known: day 1 was a Monday. The week's seven days are TimeUnit's.
/// <summary>The day count every calendar's dates fall on, and the weekday of a day on it.</summary>
public static class RataDie
{
    /// <summary>The day of the week the count's epoch, day 1, fell on: a Monday.</summary>
    public const DayOfWeek EpochWeekday = DayOfWeek.Monday;

    private const long DaysPerWeek = TimeUnit.Week.Ticks / TimeUnit.Day.Ticks;

    /// <summary>Finds the day of the week a day of the count falls on.</summary>
    /// <param name="dayNumber">The day's number on the count, zero or less before day 1.</param>
    /// <returns>The day of the week, counted on from day 1's Monday.</returns>
    public static DayOfWeek WeekdayOf(long dayNumber)
    {
        // DayOfWeek numbers the days Sunday 0 to Saturday 6, so day 1's number plus the days since
        // it, taken round the week, is the day's. A day before day 1 leaves a negative remainder,
        // which one more week brings into the week.
        var weekday = (dayNumber - GregorianCalendar.Epoch + (long)EpochWeekday) % DaysPerWeek;
        if (weekday < 0)
        {
            weekday += DaysPerWeek;
        }

        return (DayOfWeek)weekday;
    }
}
