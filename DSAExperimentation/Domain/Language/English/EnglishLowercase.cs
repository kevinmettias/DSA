using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Language.English;

// The 26 lowercase English letters as slots 0 to 25, the alphabet most LeetCode strings are written
// in: count tables, bitmasks and tries index letters by it. They are one run of character codes, so
// a slot is the letter's distance from 'a'. How many there are is EnglishLetters.Size: the count is
// the alphabet's, and each case has as many letters because English pairs them one for one.
/// <summary>The 26 lowercase English letters, 'a' in slot 0 through 'z' in slot 25.</summary>
public readonly struct EnglishLowercase : IAlphabet
{
    private const char FirstLetter = 'a';

    private static LetterRun Run => new(FirstLetter, EnglishLetters.Size);

    static int IAlphabet.Size => EnglishLetters.Size;

    /// <summary>Finds the slot a lowercase letter takes.</summary>
    /// <param name="letter">A lowercase English letter, 'a' through 'z'.</param>
    /// <returns>The letter's slot, 0 for 'a' through 25 for 'z'.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is outside 'a' through 'z'.</exception>
    public static int IndexOf(char letter) => Run.IndexOf(letter);

    /// <summary>Finds the slot a character takes, if it is a lowercase English letter.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The letter's slot; meaningful only when this returns true.</param>
    /// <returns>True for 'a' through 'z'; otherwise false.</returns>
    public static bool TryIndexOf(char letter, out int index) => Run.TryIndexOf(letter, out index);

    /// <summary>Finds the lowercase letter a slot holds.</summary>
    /// <param name="index">A slot, 0 through 25.</param>
    /// <returns>The slot's letter, 'a' for 0 through 'z' for 25.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside [0, 26).</exception>
    public static char LetterAt(int index) => Run.LetterAt(index);
}
