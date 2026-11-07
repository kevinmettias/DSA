using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Ordinals;

// English names a fraction's parts by its denominator's ordinal (a third, three fifths, two
// hundredths), plural with an s, except two denominators, each a named value: two gives a half and
// halves, and four a quarter and quarters, beside the ordinal fourth that American English also
// uses. Only the denominators two and four themselves are irregular, so 1/22 is a twenty-second.
/// <summary>English's fraction words: the two denominators with words of their own.</summary>
public static class EnglishFractionWords
{
    private const char PluralEnding = 's';

    /// <summary>Gets half, halves: the parts of two.</summary>
    public static FractionWord Half { get; } = new(EnglishNumberWords.Two, "half", "halves");

    /// <summary>Gets quarter, quarters: the parts of four.</summary>
    public static FractionWord Quarter { get; } =
        new(EnglishNumberWords.Four, "quarter", "quarters");

    /// <summary>Gets the two denominators with words of their own, in increasing order.</summary>
    public static ImmutableArray<FractionWord> Irregular { get; } =
        [.. new[] { Half, Quarter }.OrderBy(fraction => fraction.Denominator.Value)];

    /// <summary>The word for one part of a denominator, given the denominator's ordinal.</summary>
    /// <param name="denominator">The denominator, at least two.</param>
    /// <param name="ordinal">The denominator's ordinal, such as fifth.</param>
    /// <returns>Half, quarter, or the ordinal itself.</returns>
    public static string SingularOf(long denominator, string ordinal)
    {
        foreach (var irregular in Irregular)
        {
            if (irregular.Denominator.Value == denominator)
            {
                return irregular.Singular;
            }
        }

        return ordinal;
    }

    /// <summary>
    /// The word for several parts of a denominator, given the denominator's ordinal.
    /// </summary>
    /// <param name="denominator">The denominator, at least two.</param>
    /// <param name="ordinal">The denominator's ordinal, such as fifth.</param>
    /// <returns>Halves, quarters, or the ordinal with an s.</returns>
    public static string PluralOf(long denominator, string ordinal)
    {
        foreach (var irregular in Irregular)
        {
            if (irregular.Denominator.Value == denominator)
            {
                return irregular.Plural;
            }
        }

        return ordinal + PluralEnding;
    }
}
