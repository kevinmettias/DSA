namespace DSAExperimentation.Domain.Science.Time;

// A unit of time as a witness: its name and its length are static, so code generic over a unit reads
// them as constants. TimeUnit holds the units.
/// <summary>A unit of time: its name and its length.</summary>
public interface ITimeUnit
{
    /// <summary>Gets the unit's name, lowercase and singular, such as "minute".</summary>
    static abstract string Name { get; }

    /// <summary>Gets how long one of the unit lasts, in TimeSpan's 100-nanosecond ticks.</summary>
    static abstract long Ticks { get; }
}
