using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text;

namespace DSAExperimentation.Domain.Language.Scripts.Latin;

// The Latin script's letters as basic letters and the diacritics written on them. The basic letters
// are the ISO basic Latin alphabet's 26, the ones ASCII holds, which English writes with no mark.
// English's letters never come here, since its alphabets are runs of character codes
// (Language/English); only an alphabet that writes accented letters decomposes them. Decomposition
// is Unicode's canonical one, string.Normalize with NormalizationForm.FormD, which splits é into e
// and U+0301; each combining mark is then looked up among the named diacritics, gathered once by
// their marks. Some letters carry nothing Unicode calls a diacritic: ø and ł are letters with a
// stroke, and ß, æ and œ letters of their own, so none decomposes. Neither does a letter whose base
// is one of them, such as ǿ, nor an accented letter of another script, such as Greek ά: their bases
// are no basic Latin letters. Taking a letter apart allocates a string, so it is for reading an
// alphabet once, never for a hot loop.
/// <summary>The Latin script's diacritics, and its letters taken apart into basic letters and marks.</summary>
public static class LatinScript
{
    private const string NotDecomposableMessage =
        "Expected a basic Latin letter, or one with named diacritics written on it.";

    private static readonly ImmutableArray<Diacritic> AllDiacritics =
    [
        Diacritic.Grave, Diacritic.Acute, Diacritic.Circumflex, Diacritic.Tilde, Diacritic.Macron,
        Diacritic.Breve, Diacritic.DotAbove, Diacritic.Diaeresis, Diacritic.HookAbove,
        Diacritic.RingAbove, Diacritic.DoubleAcute, Diacritic.Caron, Diacritic.Horn,
        Diacritic.DotBelow, Diacritic.CommaBelow, Diacritic.Cedilla, Diacritic.Ogonek,
    ];

    private static readonly FrozenDictionary<char, Diacritic> DiacriticsByMark =
        AllDiacritics.ToFrozenDictionary(diacritic => diacritic.CombiningMark);

    /// <summary>The named diacritics, in the order of their combining marks' code points.</summary>
    public static ImmutableArray<Diacritic> Diacritics => AllDiacritics;

    /// <summary>Tests whether a character is one of the 26 basic Latin letters, in either case.</summary>
    /// <param name="letter">The character to test.</param>
    /// <returns>True for 'a' through 'z' and 'A' through 'Z'; otherwise false.</returns>
    public static bool IsBasicLetter(char letter) => char.IsAsciiLetter(letter);

    /// <summary>Takes a Latin letter apart into its basic letter and the diacritics written on it.</summary>
    /// <param name="letter">A basic Latin letter, or one with named diacritics, such as 'é'.</param>
    /// <returns>The basic letter and its diacritics, such as 'e' and the acute accent.</returns>
    /// <exception cref="ArgumentException"><paramref name="letter"/> does not decompose into a basic letter and named diacritics.</exception>
    public static DecomposedLetter Decompose(char letter)
    {
        if (!TryDecompose(letter, out var decomposed))
        {
            throw new ArgumentException(NotDecomposableMessage, nameof(letter));
        }

        return decomposed;
    }

    /// <summary>Takes a character apart into a basic letter and diacritics, if it is a Latin letter made of them.</summary>
    /// <param name="letter">The character to take apart.</param>
    /// <param name="decomposed">The basic letter and its diacritics; meaningful only when this returns true.</param>
    /// <returns>True when Unicode decomposes <paramref name="letter"/> into a basic Latin letter followed only by named diacritics; otherwise false.</returns>
    public static bool TryDecompose(char letter, out DecomposedLetter decomposed)
    {
        decomposed = default;
        var parts = PartsOnABasicLetter(letter);

        if (parts is null || !TryNameMarks(parts.AsSpan(1), out var diacritics))
        {
            return false;
        }

        decomposed = new DecomposedLetter(parts[0], diacritics);
        return true;
    }

    // A letter's canonical decomposition when it begins with a basic letter, or null. Only a letter
    // is a basic letter with marks, and some characters that are not, half of a surrogate pair or
    // the noncharacter U+FFFE, are ones Unicode refuses to normalize.
    private static string? PartsOnABasicLetter(char letter)
    {
        if (!char.IsLetter(letter))
        {
            return null;
        }

        var parts = letter.ToString().Normalize(NormalizationForm.FormD);

        if (!IsBasicLetter(parts[0]))
        {
            return null;
        }

        return parts;
    }

    // The named diacritic of each combining mark, in order, or false when a mark is none of them.
    private static bool TryNameMarks(ReadOnlySpan<char> marks, out ImmutableArray<Diacritic> diacritics)
    {
        var named = ImmutableArray.CreateBuilder<Diacritic>(marks.Length);

        foreach (var mark in marks)
        {
            if (!DiacriticsByMark.TryGetValue(mark, out var diacritic))
            {
                diacritics = default;
                return false;
            }

            named.Add(diacritic);
        }

        diacritics = named.MoveToImmutable();
        return true;
    }
}
