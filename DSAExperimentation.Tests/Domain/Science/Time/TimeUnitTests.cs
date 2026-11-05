using DSAExperimentation.Domain.Science.Time;

namespace DSAExperimentation.Tests.Domain.Science.Time;

// Each unit's length is the fact, so the tests state it as how many of the next smaller unit make it
// up, which chains every length back to the microsecond, and read each name.
public sealed partial class TimeUnitTests
{
    [Fact]
    public void Count_EachUnitInTheNextSmaller_ChainsBackToTheMicrosecond() =>
        Assert.Equal<long>(
            [1_000, 1_000, 60, 60, 24, 7],
            [
                TimeUnit.Count<TimeUnit.Millisecond, TimeUnit.Microsecond>(),
                TimeUnit.Count<TimeUnit.Second, TimeUnit.Millisecond>(),
                TimeUnit.Count<TimeUnit.Minute, TimeUnit.Second>(),
                TimeUnit.Count<TimeUnit.Hour, TimeUnit.Minute>(),
                TimeUnit.Count<TimeUnit.Day, TimeUnit.Hour>(),
                TimeUnit.Count<TimeUnit.Week, TimeUnit.Day>(),
            ]);

    [Fact]
    public void Count_InSeconds_IsTheUnitsLengthInSeconds() =>
        Assert.Equal<long>(
            [60, 3_600, 86_400, 604_800],
            [
                TimeUnit.Count<TimeUnit.Minute, TimeUnit.Second>(),
                TimeUnit.Count<TimeUnit.Hour, TimeUnit.Second>(),
                TimeUnit.Count<TimeUnit.Day, TimeUnit.Second>(),
                TimeUnit.Count<TimeUnit.Week, TimeUnit.Second>(),
            ]);

    [Fact]
    public void Name_EachUnit_IsItsEnglishName() =>
        Assert.Equal(
            ["microsecond", "millisecond", "second", "minute", "hour", "day", "week"],
            [
                TimeUnit.Microsecond.Name, TimeUnit.Millisecond.Name, TimeUnit.Second.Name, TimeUnit.Minute.Name,
                TimeUnit.Hour.Name, TimeUnit.Day.Name, TimeUnit.Week.Name,
            ]);

    [Fact]
    public void Ticks_Second_IsOneSecond() => Assert.Equal(TimeUnit.Second.Ticks, TimeSpan.FromSeconds(1).Ticks);

    [Fact]
    public void Count_UnitInItself_IsOne() => Assert.Equal(1, TimeUnit.Count<TimeUnit.Day, TimeUnit.Day>());

    [Fact]
    public void Count_LargerUnitInASmaller_Throws() =>
        Assert.Throws<ArgumentException>(() => TimeUnit.Count<TimeUnit.Second, TimeUnit.Minute>());

    [Fact]
    public void Count_UnitThatDoesNotFitWholly_Throws() =>
        Assert.Throws<ArgumentException>(() => TimeUnit.Count<NinetySeconds, TimeUnit.Minute>());

    // A unit no whole number of minutes long.
    private readonly struct NinetySeconds : ITimeUnit
    {
        public static string Name => "ninety seconds";

        public static long Ticks => 90 * TimeSpan.TicksPerSecond;
    }
}
