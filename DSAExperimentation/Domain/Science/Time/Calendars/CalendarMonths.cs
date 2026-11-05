using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Science.Time.Calendars;

// The twelve months the Julian reform gave the Roman year and the Gregorian reform kept, each a
// named value carrying its number and its days in a common year. Each length is stated once, as a
// constant, and the common year is their sum, so the compiler writes 365 and no count is stated
// beside the months. A leap year adds one day to February, its 29th. The tables are built from the
// named values, each month placed by its own number, so the order the months are listed in cannot
// misplace one.
/// <summary>The twelve months, the days of a common and a leap year, and the leap month.</summary>
public static class CalendarMonths
{
    private const int JanuaryDays = 31;
    private const int FebruaryDays = 28;
    private const int MarchDays = 31;
    private const int AprilDays = 30;
    private const int MayDays = 31;
    private const int JuneDays = 30;
    private const int JulyDays = 31;
    private const int AugustDays = 31;
    private const int SeptemberDays = 30;
    private const int OctoberDays = 31;
    private const int NovemberDays = 30;
    private const int DecemberDays = 31;

    private const int FebruaryNumber = 2;

    // A leap year adds one day, the leap day.
    private const int LeapDays = 1;

    /// <summary>The days of a common year, the twelve months' days together: 365.</summary>
    public const int DaysPerCommonYear = JanuaryDays + FebruaryDays + MarchDays + AprilDays
        + MayDays + JuneDays + JulyDays + AugustDays + SeptemberDays + OctoberDays + NovemberDays
        + DecemberDays;

    /// <summary>The days of a leap year, a common year's and the leap day: 366.</summary>
    public const int DaysPerLeapYear = DaysPerCommonYear + LeapDays;

    /// <summary>February's number: a leap year lengthens it by the leap day, its 29th.</summary>
    public const int LeapMonth = FebruaryNumber;

    /// <summary>January, month 1, 31 days.</summary>
    public static CalendarMonth January { get; } = new(1, JanuaryDays);

    /// <summary>February, month 2, 28 days in a common year and 29 in a leap year.</summary>
    public static CalendarMonth February { get; } = new(FebruaryNumber, FebruaryDays);

    /// <summary>March, month 3, 31 days.</summary>
    public static CalendarMonth March { get; } = new(3, MarchDays);

    /// <summary>April, month 4, 30 days.</summary>
    public static CalendarMonth April { get; } = new(4, AprilDays);

    /// <summary>May, month 5, 31 days.</summary>
    public static CalendarMonth May { get; } = new(5, MayDays);

    /// <summary>June, month 6, 30 days.</summary>
    public static CalendarMonth June { get; } = new(6, JuneDays);

    /// <summary>July, month 7, 31 days.</summary>
    public static CalendarMonth July { get; } = new(7, JulyDays);

    /// <summary>August, month 8, 31 days.</summary>
    public static CalendarMonth August { get; } = new(8, AugustDays);

    /// <summary>September, month 9, 30 days.</summary>
    public static CalendarMonth September { get; } = new(9, SeptemberDays);

    /// <summary>October, month 10, 31 days.</summary>
    public static CalendarMonth October { get; } = new(10, OctoberDays);

    /// <summary>November, month 11, 30 days.</summary>
    public static CalendarMonth November { get; } = new(11, NovemberDays);

    /// <summary>December, month 12, 31 days.</summary>
    public static CalendarMonth December { get; } = new(12, DecemberDays);

    /// <summary>The twelve months, each at the index one below its number.</summary>
    public static ImmutableArray<CalendarMonth> All { get; } = ByNumber(
        January, February, March, April, May, June,
        July, August, September, October, November, December);

    /// <summary>The days of a common year before each month starts, January's 0 first.</summary>
    public static ImmutableArray<int> DaysBeforeMonth { get; } = RunningTotalBefore(All);

    private static ImmutableArray<CalendarMonth> ByNumber(params CalendarMonth[] months)
        => [.. months.OrderBy(month => month.Number)];

    // Each month's days added up over the months before it, January's none.
    private static ImmutableArray<int> RunningTotalBefore(ImmutableArray<CalendarMonth> months)
    {
        var totals = ImmutableArray.CreateBuilder<int>(months.Length);
        var daysSoFar = 0;
        foreach (var month in months)
        {
            totals.Add(daysSoFar);
            daysSoFar += month.DaysInCommonYear;
        }

        return totals.MoveToImmutable();
    }
}
