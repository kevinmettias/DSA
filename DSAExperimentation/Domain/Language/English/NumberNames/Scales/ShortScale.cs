using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Scales;

// The short scale, English's today: each scale word a thousand times the one before, so a billion
// is a thousand million. Its groups are the thousands grouping a numeral is written in, and its
// words EnglishNumberWords' short scale, thousand to quintillion.
/// <summary>The short scale: thousand, million, billion, each a thousand times the last.</summary>
public readonly struct ShortScale : INumberScale
{
    /// <inheritdoc/>
    public static int FirstGroupDigits => ThousandsGrouping.FirstGroupDigits;

    /// <inheritdoc/>
    public static int LaterGroupDigits => ThousandsGrouping.LaterGroupDigits;

    /// <inheritdoc/>
    public static ImmutableArray<NumberWord> GroupWords => EnglishNumberWords.ShortScale;
}
