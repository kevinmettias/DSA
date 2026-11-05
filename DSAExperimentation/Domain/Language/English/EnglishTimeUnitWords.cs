using System.Collections.Immutable;
using DSAExperimentation.Domain.Science.Time;

namespace DSAExperimentation.Domain.Language.English;

// English's words for the units of time, each a named value carrying the length of the unit it
// names, read from TimeUnit's constants. The units and their lengths are Science/Time's, and a
// language only names them, as English's number words name numbers (EnglishNumberWords). The words
// are lowercase and singular, as written within a sentence; a problem whose notation spells a unit
// some other way, as LC 1348's frequency names do, keeps its spelling in its own folder. The list
// is sorted by length as it is built, so the order the words are written in below cannot misplace
// one.
/// <summary>English's words for the units of time, the microsecond to the week.</summary>
public static class EnglishTimeUnitWords
{
    /// <summary>A microsecond, a millionth of a second.</summary>
    public static TimeUnitWord Microsecond { get; } =
        new(TimeUnit.Microsecond.Ticks, "microsecond");

    /// <summary>A millisecond, a thousandth of a second.</summary>
    public static TimeUnitWord Millisecond { get; } =
        new(TimeUnit.Millisecond.Ticks, "millisecond");

    /// <summary>A second.</summary>
    public static TimeUnitWord Second { get; } = new(TimeUnit.Second.Ticks, "second");

    /// <summary>A minute.</summary>
    public static TimeUnitWord Minute { get; } = new(TimeUnit.Minute.Ticks, "minute");

    /// <summary>An hour.</summary>
    public static TimeUnitWord Hour { get; } = new(TimeUnit.Hour.Ticks, "hour");

    /// <summary>A day.</summary>
    public static TimeUnitWord Day { get; } = new(TimeUnit.Day.Ticks, "day");

    /// <summary>A week.</summary>
    public static TimeUnitWord Week { get; } = new(TimeUnit.Week.Ticks, "week");

    /// <summary>Every unit's word, from the shortest unit to the longest.</summary>
    public static ImmutableArray<TimeUnitWord> All { get; } = SortedByLength(
        Microsecond, Millisecond, Second, Minute, Hour, Day, Week);

    private static ImmutableArray<TimeUnitWord> SortedByLength(params TimeUnitWord[] words)
        => [.. words.OrderBy(word => word.Ticks)];
}
