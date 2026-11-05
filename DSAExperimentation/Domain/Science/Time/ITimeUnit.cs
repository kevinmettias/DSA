namespace DSAExperimentation.Domain.Science.Time;

// A unit of time as a witness: its length is static, so code generic over a unit reads it as a
// constant. TimeUnit holds the common units; a unit of any other fixed length is a struct that
// states its own, as a test's ninety seconds does. A unit's name is a language's, not the unit's,
// so English's words for the units are Language/English's EnglishTimeUnitWords.
/// <summary>A unit of time of a fixed length.</summary>
public interface ITimeUnit
{
    /// <summary>Gets how long one of the unit lasts, in TimeSpan's 100-nanosecond ticks.</summary>
    static abstract long Ticks { get; }
}
