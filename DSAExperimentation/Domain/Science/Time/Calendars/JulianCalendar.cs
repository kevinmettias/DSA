namespace DSAExperimentation.Domain.Science.Time.Calendars;

// The calendar Julius Caesar's reform began in 45 BC: a leap year every four years, century years
// included, so its leap-century cycle is the century itself and no leap year is dropped. Its year
// averages 365.25 days. Run back to year 1, its 1 January falls on day -1 of the shared count, two
// days before the Gregorian calendar's: the two name the same days from 1 March 200 to 28 February
// 300, and before that the Julian calendar's leap days of 100 and 200, which the Gregorian rule
// drops, put its dates two days earlier. Each member stays a constant for a caller that names one;
// the calendar contract reads them.
/// <summary>The Julian calendar: a leap year every four years, century years included.</summary>
public readonly struct JulianCalendar : ICalendar
{
    /// <summary>A leap year every four years.</summary>
    public const int LeapYearCycle = 4;

    /// <summary>A century year every hundred years.</summary>
    public const int CenturyCycle = 100;

    /// <summary>The century itself, since every century year stays a leap year.</summary>
    public const int LeapCenturyCycle = CenturyCycle;

    /// <summary>The day number of 1 January of year 1 on the shared count, -1.</summary>
    public const long Epoch = -1;

    static int ICalendar.LeapYearCycle => LeapYearCycle;

    static int ICalendar.CenturyCycle => CenturyCycle;

    static int ICalendar.LeapCenturyCycle => LeapCenturyCycle;

    static long ICalendar.Epoch => Epoch;
}
