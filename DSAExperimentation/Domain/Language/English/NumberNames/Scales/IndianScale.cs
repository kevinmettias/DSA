using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Scales;

// Indian English's scale: a thousand, then each word a hundred times the one before, a lakh (a
// hundred thousand, 1,00,000) and a crore (a hundred lakh, 1,00,00,000). Its groups are the Indian
// grouping a numeral is written in, three digits then two. Above a crore the count of crores is
// read again in the same scale, so 10^12 is one lakh crore; the older words for higher groups, arab
// and kharab, are rare in English and are not offered.
/// <summary>Indian English's scale: thousand, lakh, crore.</summary>
public readonly struct IndianScale : INumberScale
{
    /// <summary>Gets a lakh, a hundred thousand: 1,00,000.</summary>
    public static NumberWord Lakh { get; } =
        new(EnglishNumberWords.Hundred.Value * EnglishNumberWords.Thousand.Value, "lakh");

    /// <summary>Gets a crore, a hundred lakh: 1,00,00,000.</summary>
    public static NumberWord Crore { get; } =
        new(EnglishNumberWords.Hundred.Value * Lakh.Value, "crore");

    /// <inheritdoc/>
    public static int FirstGroupDigits => IndianGrouping.FirstGroupDigits;

    /// <inheritdoc/>
    public static int LaterGroupDigits => IndianGrouping.LaterGroupDigits;

    /// <inheritdoc/>
    public static ImmutableArray<NumberWord> GroupWords { get; } =
        [EnglishNumberWords.Thousand, Lakh, Crore];
}
