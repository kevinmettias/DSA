using System.Runtime.CompilerServices;

namespace DSAExperimentation.Domain.Science.Time;

// Units of time, each a named struct carrying its length. A microsecond, a millisecond, a second, a
// minute, an hour and a day are each an exact number of .NET's 100-nanosecond ticks, and their
// lengths are TimeSpan's own constants, so the platform stays the one source of those numbers; a
// week is seven days, the one count stated here. The day is the unit of 86,400 seconds the SI
// accepts, not a civil day: a change of daylight saving makes a civil day 23 or 25 hours, and a
// leap second makes a UTC day 86,401 seconds. A month and a year are not a fixed length: a calendar
// decides them, so they are its units, and Calendars/ states them (Calendar.MonthLength and
// YearLength). A nanosecond is a hundredth of a tick, so it is not offered either. A unit's name is
// a language's, so English's words for these are Language/English's (EnglishTimeUnitWords), and how
// a problem names its units is its own notation, mapped onto these.
//
// Every length is fixed, so each unit states its length as a C# constant (the owner's ruling of
// 2026-11-04): a caller that knows its units divides one constant by another, Minute.Ticks / Second.Ticks,
// and the compiler writes 60. A caller generic over its units has Count<TUnit, TIn>, which reads the same
// constants through ITimeUnit and folds to the count once inlined. LC 1348 first called Count from its
// switch on a frequency's name: each arm folded, but the switch's IL had grown past what the JIT would
// inline into the query, which then paid a call on every query and read 7-21% slower.
/// <summary>Units of time, each carrying its length, and the count of one unit in another.</summary>
public static class TimeUnit
{
    private const int DaysPerWeek = 7;

    private const string NotAWholeCountMessage = "Expected a unit that fits a whole number of times into this one.";

    /// <summary>Counts how many of one unit make up another.</summary>
    /// <typeparam name="TUnit">The unit to measure.</typeparam>
    /// <typeparam name="TIn">The unit to count in, which must fit a whole number of times into <typeparamref name="TUnit"/>.</typeparam>
    /// <returns>The count, such as 60 for a minute in seconds.</returns>
    /// <exception cref="ArgumentException"><typeparamref name="TIn"/> does not fit a whole number of times into <typeparamref name="TUnit"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Count<TUnit, TIn>()
        where TUnit : struct, ITimeUnit
        where TIn : struct, ITimeUnit
    {
        if (TUnit.Ticks % TIn.Ticks != 0)
        {
            ThrowNotAWholeCount<TIn>();
        }

        return TUnit.Ticks / TIn.Ticks;
    }

    // Kept out of Count so that Count stays small enough to inline, where its two lengths are constants
    // and the test above folds away. With the throw in it the JIT judged Count unprofitable and called
    // it; without, it still called Count from a switch arm profiling found cold, so Count asks to be
    // inlined everywhere, and every arm compiles to its count.
    private static void ThrowNotAWholeCount<TIn>() => throw new ArgumentException(NotAWholeCountMessage, nameof(TIn));

    /// <summary>A microsecond, a millionth of a second.</summary>
    public readonly struct Microsecond : ITimeUnit
    {
        /// <summary>How long one microsecond lasts, in TimeSpan's ticks.</summary>
        public const long Ticks = TimeSpan.TicksPerMicrosecond;

        static long ITimeUnit.Ticks => Ticks;
    }

    /// <summary>A millisecond, a thousandth of a second.</summary>
    public readonly struct Millisecond : ITimeUnit
    {
        /// <summary>How long one millisecond lasts, in TimeSpan's ticks.</summary>
        public const long Ticks = TimeSpan.TicksPerMillisecond;

        static long ITimeUnit.Ticks => Ticks;
    }

    /// <summary>A second, the SI unit of time.</summary>
    public readonly struct Second : ITimeUnit
    {
        /// <summary>How long one second lasts, in TimeSpan's ticks.</summary>
        public const long Ticks = TimeSpan.TicksPerSecond;

        static long ITimeUnit.Ticks => Ticks;
    }

    /// <summary>A minute, 60 seconds.</summary>
    public readonly struct Minute : ITimeUnit
    {
        /// <summary>How long one minute lasts, in TimeSpan's ticks.</summary>
        public const long Ticks = TimeSpan.TicksPerMinute;

        static long ITimeUnit.Ticks => Ticks;
    }

    /// <summary>An hour, 60 minutes.</summary>
    public readonly struct Hour : ITimeUnit
    {
        /// <summary>How long one hour lasts, in TimeSpan's ticks.</summary>
        public const long Ticks = TimeSpan.TicksPerHour;

        static long ITimeUnit.Ticks => Ticks;
    }

    /// <summary>A day, 24 hours: the SI-accepted unit, not a civil day that daylight saving or a leap second lengthens.</summary>
    public readonly struct Day : ITimeUnit
    {
        /// <summary>How long one day lasts, in TimeSpan's ticks.</summary>
        public const long Ticks = TimeSpan.TicksPerDay;

        static long ITimeUnit.Ticks => Ticks;
    }

    /// <summary>A week, seven days.</summary>
    public readonly struct Week : ITimeUnit
    {
        /// <summary>How long one week lasts, in TimeSpan's ticks.</summary>
        public const long Ticks = TimeSpan.TicksPerDay * DaysPerWeek;

        static long ITimeUnit.Ticks => Ticks;
    }
}
