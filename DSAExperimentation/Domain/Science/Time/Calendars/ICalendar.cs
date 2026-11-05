namespace DSAExperimentation.Domain.Science.Time.Calendars;

// A calendar of the Julian family: the twelve Roman months (CalendarMonths), a leap day added to
// February, and a leap year every few years except in some century years. The Julian calendar has a
// leap year every four years, century years included; the Gregorian reform of 1582 kept that and
// made a century year a leap year only when its number is a multiple of 400. So one rule with three
// cycles states both: a year is a leap year when the leap-year cycle divides it, unless the century
// cycle does and the leap-century cycle does not. Each cycle divides the next, and every century
// year is a leap year exactly when the last two are equal, as in the Julian calendar. A calendar
// also says where its dates fall on the day count every calendar shares (RataDie), its epoch.
// Calendars whose months or leap rule differ (the Hebrew, the Islamic, the Revised Julian's
// 900-year rule) are not of this family and are not offered. Every member is a static abstract
// member of a struct, so code generic over a calendar reads them as constants.
/// <summary>A calendar of the Julian family: its leap-year rule and its epoch.</summary>
public interface ICalendar
{
    /// <summary>Gets the leap-year cycle: a leap year comes every this many years.</summary>
    static abstract int LeapYearCycle { get; }

    /// <summary>Gets the century cycle: a year it divides is a century year.</summary>
    static abstract int CenturyCycle { get; }

    /// <summary>Gets the leap-century cycle: a century year it divides stays a leap year.</summary>
    static abstract int LeapCenturyCycle { get; }

    /// <summary>Gets the day number on the shared count (RataDie) of 1 January of year 1.</summary>
    static abstract long Epoch { get; }
}
