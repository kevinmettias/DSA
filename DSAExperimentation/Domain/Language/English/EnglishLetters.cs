using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Language.English;

// The 26 English letters in either case, 'a' and 'A' sharing slot 0: English pairs its capitals with
// its lowercase letters one for one, so case folds exactly (CaseFolded). It also carries English's
// numbering of its letters, A as 1 through Z as 26, the notation LC 91 decodes and LC 2156 values
// letters by. That numbering is English's and not every alphabet's - Greek and Hebrew numerals give
// kappa and kaf the value 20 - so it lives here rather than on the alphabet contract.
/// <summary>The 26 English letters with case set aside, and their numbers, A as 1 through Z as 26.</summary>
public readonly struct EnglishLetters : IAlphabet
{
    /// <summary>The number of letters, 26, which is also the last letter's number.</summary>
    public const int Size = 26;

    static int IAlphabet.Size => Size;

    /// <summary>Finds the slot a letter takes in either case.</summary>
    /// <param name="letter">An English letter, lowercase or capital.</param>
    /// <returns>The letter's slot, 0 for 'a' or 'A' through 25 for 'z' or 'Z'.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not an English letter.</exception>
    public static int IndexOf(char letter) => CaseFolded<EnglishLowercase, EnglishUppercase>.IndexOf(letter);

    /// <summary>Finds the slot a character takes, if it is an English letter in either case.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The letter's slot; meaningful only when this returns true.</param>
    /// <returns>True for 'a' through 'z' and 'A' through 'Z'; otherwise false.</returns>
    public static bool TryIndexOf(char letter, out int index) =>
        CaseFolded<EnglishLowercase, EnglishUppercase>.TryIndexOf(letter, out index);

    /// <summary>Finds the letter a slot holds, in lowercase.</summary>
    /// <param name="index">A slot, 0 through 25.</param>
    /// <returns>The slot's lowercase letter, 'a' for 0 through 'z' for 25.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside [0, 26).</exception>
    public static char LetterAt(int index) => CaseFolded<EnglishLowercase, EnglishUppercase>.LetterAt(index);

    /// <summary>A letter's number, A or a as 1 through Z or z as 26.</summary>
    /// <param name="letter">An English letter, lowercase or capital.</param>
    /// <returns>The letter's number.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not an English letter.</exception>
    public static int NumberOf(char letter) => IndexOf(letter) + 1;
}
