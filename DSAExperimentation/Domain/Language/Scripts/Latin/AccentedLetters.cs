using System.Collections.Frozen;
using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Scripts.Latin;

// An alphabet's accented letters taken apart once: each of TAlphabet's accented letters, and its
// capital, is decomposed through LatinScript when the type is first used, into a table from the
// letter to its basic letter and marks. A question about a letter is then one lookup, with no
// normalization; this is the only path that reaches Unicode's decomposition, and English's
// alphabets, which write no accented letter, never take it. Folding a letter to its base is how
// text is compared with accents set aside, so "café" and "cafe" fold alike.
/// <summary>An accented alphabet's letters with their basic letters and diacritics, looked up in either case.</summary>
/// <typeparam name="TAlphabet">The alphabet, such as <see cref="SpanishAlphabet"/>.</typeparam>
public static class AccentedLetters<TAlphabet>
    where TAlphabet : struct, IAccentedAlphabet
{
    private static readonly FrozenDictionary<char, DecomposedLetter> Decompositions =
        DecomposeEachLetter();

    /// <summary>Tests whether a character is one of the alphabet's accented letters, in either case.</summary>
    /// <param name="letter">The character to test.</param>
    /// <returns>True for one of the alphabet's accented letters, lowercase or capital; otherwise false.</returns>
    public static bool IsAccented(char letter) => Decompositions.ContainsKey(letter);

    /// <summary>Folds one of the alphabet's accented letters to its basic letter, keeping its case.</summary>
    /// <param name="letter">The character to fold.</param>
    /// <returns>The basic letter, 'e' for 'é' and 'E' for 'É'; any other character as it is.</returns>
    public static char BaseOf(char letter)
    {
        if (!Decompositions.TryGetValue(letter, out var decomposed))
        {
            return letter;
        }

        return decomposed.BaseLetter;
    }

    /// <summary>The diacritics written on one of the alphabet's accented letters.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <returns>The letter's diacritics in Unicode's canonical order; empty for any other character.</returns>
    public static ImmutableArray<Diacritic> DiacriticsOf(char letter)
    {
        if (!Decompositions.TryGetValue(letter, out var decomposed))
        {
            return [];
        }

        return decomposed.Diacritics;
    }

    private static FrozenDictionary<char, DecomposedLetter> DecomposeEachLetter() =>
        TAlphabet.AccentedLetters
            .SelectMany(letter => new[] { letter, char.ToUpperInvariant(letter) })
            .Distinct()
            .ToFrozenDictionary(letter => letter, LatinScript.Decompose);
}
