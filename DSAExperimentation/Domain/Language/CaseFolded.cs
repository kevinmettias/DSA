using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Language;

// A cased writing system's letters with case set aside: a letter and its capital take the same slot,
// so a count, a set or a trie over this alphabet treats 'A' and 'a' as one letter. An AlphabetTrie
// over it is a case-insensitive map, and LetterAt gives the lowercase form. The lowercase run is
// tried first, since most text is lowercase.
//
// That is exact only where case pairs letter for letter, as in English and Russian: the two
// alphabets must have one size, and slot i of each must be the two forms of one letter, which a test
// checks for each pairing. German's ß capitalizes to two letters and Turkish pairs i with İ and ı
// with I, so neither is such a pairing (ARCHITECTURE §17.6, a fact names what it assumes).
/// <summary>An alphabet whose letters are the slots two case forms share.</summary>
/// <typeparam name="TLower">The lowercase letters.</typeparam>
/// <typeparam name="TUpper">The capitals, in the same slots as their lowercase forms.</typeparam>
public readonly struct CaseFolded<TLower, TUpper> : IAlphabet
    where TLower : struct, IAlphabet
    where TUpper : struct, IAlphabet
{
    private const string NotALetterMessage = "Expected a letter of the alphabet in either case.";

    /// <summary>How many letters there are, counting a letter and its capital once.</summary>
    public static int Size => TLower.Size;

    /// <summary>Finds the slot a letter takes in either case.</summary>
    /// <param name="letter">A letter, lowercase or capital.</param>
    /// <returns>The slot the letter shares with its other case.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is a letter in neither case.</exception>
    public static int IndexOf(char letter)
    {
        if (!TryIndexOf(letter, out var index))
        {
            ThrowNotALetter(letter);
        }

        return index;
    }

    private static void ThrowNotALetter(char letter) =>
        throw new ArgumentOutOfRangeException(nameof(letter), letter, NotALetterMessage);

    /// <summary>Finds the slot a character takes, if it is a letter in either case.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The slot it shares with its other case; meaningful only when this returns true.</param>
    /// <returns>True when <paramref name="letter"/> is a letter in either case; otherwise false.</returns>
    public static bool TryIndexOf(char letter, out int index) =>
        TLower.TryIndexOf(letter, out index) || TUpper.TryIndexOf(letter, out index);

    /// <summary>Finds the letter a slot holds, in its lowercase form.</summary>
    /// <param name="index">A slot, from 0 to <see cref="Size"/> - 1.</param>
    /// <returns>The slot's lowercase letter.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a slot of the alphabet.</exception>
    public static char LetterAt(int index) => TLower.LetterAt(index);
}
