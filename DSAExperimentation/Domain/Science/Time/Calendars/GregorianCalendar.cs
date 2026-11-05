namespace DSAExperimentation.Domain.Science.Time.Calendars;

// The calendar Pope Gregory XIII's reform began in 1582, the civil calendar of most of the world.
// It kept the Julian calendar's leap year every four years and its century years, and made a
// century year a leap year only when 400 divides it, so 1600 and 2000 are leap years and 1700, 1800
// and 1900 are not. That drops three leap days every 400 years, and its year averages 365.2425
// days. The shared day count begins on its 1 January of year 1, run back before the reform. Each
// member stays a constant for a caller that names one; the calendar contract reads them.
/// <summary>The Gregorian calendar: leap years every 4 years, century years every 400.</summary>
public readonly struct GregorianCalendar : ICalendar
{
    /// <summary>A leap year every four years, as in the Julian calendar.</summary>
    public const int LeapYearCycle = JulianCalendar.LeapYearCycle;

    /// <summary>A century year every hundred years, as in the Julian calendar.</summary>
    public const int CenturyCycle = JulianCalendar.CenturyCycle;

    /// <summary>A century year stays a leap year only every 400 years.</summary>
    public const int LeapCenturyCycle = 400;

    /// <summary>The day number of 1 January of year 1, day 1 of the shared count.</summary>
    public const long Epoch = 1;

    static int ICalendar.LeapYearCycle => LeapYearCycle;

    static int ICalendar.CenturyCycle => CenturyCycle;

    static int ICalendar.LeapCenturyCycle => LeapCenturyCycle;

    static long ICalendar.Epoch => Epoch;
}
