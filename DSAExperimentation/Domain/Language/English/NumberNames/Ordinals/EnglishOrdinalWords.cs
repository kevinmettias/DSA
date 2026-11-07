using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Ordinals;

// English's ordinals are its number words with a suffix: "th" after most (fourth, eleventh,
// hundredth, millionth), and a tens word's final y becomes "ieth" (twentieth, ninetieth). Seven
// words break the rule, each a named value: first, second, third, fifth, eighth, ninth and twelfth.
// Only a whole word is irregular, so eighteen gives eighteenth, and a compound number takes the
// ordinal of its last word alone (twenty-first, one hundred second).
/// <summary>English's ordinal words: the suffix rule and its seven exceptions.</summary>
public static class EnglishOrdinalWords
{
    private const string Suffix = "th";
    private const string TensSuffix = "ieth";
    private const char TensEnding = 'y';

    /// <summary>Gets first, the ordinal of one.</summary>
    public static IrregularOrdinal First { get; } = new(EnglishNumberWords.One, "first");

    /// <summary>Gets second, the ordinal of two.</summary>
    public static IrregularOrdinal Second { get; } = new(EnglishNumberWords.Two, "second");

    /// <summary>Gets third, the ordinal of three.</summary>
    public static IrregularOrdinal Third { get; } = new(EnglishNumberWords.Three, "third");

    /// <summary>Gets fifth, the ordinal of five.</summary>
    public static IrregularOrdinal Fifth { get; } = new(EnglishNumberWords.Five, "fifth");

    /// <summary>Gets eighth, the ordinal of eight.</summary>
    public static IrregularOrdinal Eighth { get; } = new(EnglishNumberWords.Eight, "eighth");

    /// <summary>Gets ninth, the ordinal of nine.</summary>
    public static IrregularOrdinal Ninth { get; } = new(EnglishNumberWords.Nine, "ninth");

    /// <summary>Gets twelfth, the ordinal of twelve.</summary>
    public static IrregularOrdinal Twelfth { get; } = new(EnglishNumberWords.Twelve, "twelfth");

    /// <summary>Gets the seven irregular ordinals in increasing order of number.</summary>
    public static ImmutableArray<IrregularOrdinal> Irregular { get; } =
    [
        .. new[] { First, Second, Third, Fifth, Eighth, Ninth, Twelfth }
            .OrderBy(ordinal => ordinal.Cardinal.Value),
    ];

    /// <summary>The ordinal of one number word.</summary>
    /// <param name="word">The number word, such as twenty.</param>
    /// <returns>
    /// Its ordinal, such as twentieth: an exception's own, or the word with its suffix.
    /// </returns>
    public static string OrdinalOf(NumberWord word)
    {
        foreach (var irregular in Irregular)
        {
            if (irregular.Cardinal == word)
            {
                return irregular.Ordinal;
            }
        }

        if (word.Word.EndsWith(TensEnding))
        {
            var stem = word.Word[..^1];
            return stem + TensSuffix;
        }

        return word.Word + Suffix;
    }
}
