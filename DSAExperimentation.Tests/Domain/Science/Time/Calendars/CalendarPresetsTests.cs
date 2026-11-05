using DSAExperimentation.Domain.Science.Time.Calendars;

namespace DSAExperimentation.Tests.Domain.Science.Time.Calendars;

// Each calendar's rule is the fact itself, so the tests state every cycle and epoch, reading each
// both as the constant a caller names and through the contract a generic caller reads. The
// Gregorian reform kept the Julian calendar's four-year cycle and its centuries, and kept only a
// century year 400 divides as a leap year; the Julian calendar keeps every century year.
public sealed partial class CalendarPresetsTests
{
    public static TheoryData<string, long, long, long> LeapYearCycles => new()
    {
        {
            nameof(GregorianCalendar), 4, GregorianCalendar.LeapYearCycle,
            LeapYearCycleOf<GregorianCalendar>()
        },
        {
            nameof(JulianCalendar), 4, JulianCalendar.LeapYearCycle,
            LeapYearCycleOf<JulianCalendar>()
        },
    };

    public static TheoryData<string, long, long, long> CenturyCycles => new()
    {
        {
            nameof(GregorianCalendar), 100, GregorianCalendar.CenturyCycle,
            CenturyCycleOf<GregorianCalendar>()
        },
        {
            nameof(JulianCalendar), 100, JulianCalendar.CenturyCycle,
            CenturyCycleOf<JulianCalendar>()
        },
    };

    public static TheoryData<string, long, long, long> LeapCenturyCycles => new()
    {
        {
            nameof(GregorianCalendar), 400, GregorianCalendar.LeapCenturyCycle,
            LeapCenturyCycleOf<GregorianCalendar>()
        },
        {
            nameof(JulianCalendar), 100, JulianCalendar.LeapCenturyCycle,
            LeapCenturyCycleOf<JulianCalendar>()
        },
    };

    public static TheoryData<string, long, long, long> Epochs => new()
    {
        { nameof(GregorianCalendar), 1, GregorianCalendar.Epoch, EpochOf<GregorianCalendar>() },
        { nameof(JulianCalendar), -1, JulianCalendar.Epoch, EpochOf<JulianCalendar>() },
    };

    [Theory]
    [MemberData(nameof(LeapYearCycles))]
    public void LeapYearCycle_Preset_IsItsCycleAsConstantAndThroughTheContract(
        string preset, long expected, long constant, long throughContract) =>
        AssertBoth(preset, expected, constant, throughContract);

    [Theory]
    [MemberData(nameof(CenturyCycles))]
    public void CenturyCycle_Preset_IsItsCycleAsConstantAndThroughTheContract(
        string preset, long expected, long constant, long throughContract) =>
        AssertBoth(preset, expected, constant, throughContract);

    [Theory]
    [MemberData(nameof(LeapCenturyCycles))]
    public void LeapCenturyCycle_Preset_IsItsCycleAsConstantAndThroughTheContract(
        string preset, long expected, long constant, long throughContract) =>
        AssertBoth(preset, expected, constant, throughContract);

    [Theory]
    [MemberData(nameof(Epochs))]
    public void Epoch_Preset_IsItsDayAsConstantAndThroughTheContract(
        string preset, long expected, long constant, long throughContract) =>
        AssertBoth(preset, expected, constant, throughContract);

    [Fact]
    public void Epoch_Julian_IsTwoDaysBeforeTheGregorian() =>
        Assert.Equal(GregorianCalendar.Epoch - 2, JulianCalendar.Epoch);

    // The preset's name goes into the comparison so a failure says which calendar it was.
    private static void AssertBoth(
        string preset, long expected, long constant, long throughContract) =>
        Assert.Equal((preset, expected, expected), (preset, constant, throughContract));

    private static long LeapYearCycleOf<TCalendar>()
        where TCalendar : struct, ICalendar
        => TCalendar.LeapYearCycle;

    private static long CenturyCycleOf<TCalendar>()
        where TCalendar : struct, ICalendar
        => TCalendar.CenturyCycle;

    private static long LeapCenturyCycleOf<TCalendar>()
        where TCalendar : struct, ICalendar
        => TCalendar.LeapCenturyCycle;

    private static long EpochOf<TCalendar>()
        where TCalendar : struct, ICalendar
        => TCalendar.Epoch;
}
