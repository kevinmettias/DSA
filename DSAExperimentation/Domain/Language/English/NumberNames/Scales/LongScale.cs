using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Scales;

// The long scale, British English's until 1974 and most of continental Europe's still: each scale
// word a million times the one before, so a billion is a million million (10^12) and a thousand
// million is said for 10^9. Its groups are six digits, two of the thousands grouping a numeral is
// written in, and a group is read with "thousand" inside it. The words are million, billion and
// trillion, the last a long reaches; continental Europe's milliard for 10^9 is not English.
/// <summary>The long scale: million, billion, trillion, each a million times the last.</summary>
public readonly struct LongScale : INumberScale
{
    /// <summary>Gets a billion on the long scale, a million million: 10^12.</summary>
    public static NumberWord Billion { get; } =
        new(EnglishNumberWords.Million.Value * EnglishNumberWords.Million.Value, "billion");

    /// <summary>Gets a trillion on the long scale, a million billion: 10^18.</summary>
    public static NumberWord Trillion { get; } =
        new(EnglishNumberWords.Million.Value * Billion.Value, "trillion");

    /// <inheritdoc/>
    public static int FirstGroupDigits => ShortScale.FirstGroupDigits + ShortScale.LaterGroupDigits;

    /// <inheritdoc/>
    public static int LaterGroupDigits => ShortScale.LaterGroupDigits + ShortScale.LaterGroupDigits;

    /// <inheritdoc/>
    public static ImmutableArray<NumberWord> GroupWords { get; } =
        [EnglishNumberWords.Million, Billion, Trillion];
}
