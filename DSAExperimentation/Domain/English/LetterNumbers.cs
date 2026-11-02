using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Domain.English;

// The letters numbered from one, A as 1 through Z as 26, as LC 91 decodes them and LC 2156 values
// them. It is a notation people use, not the alphabet's own order: LowercaseAlphabet, in tier 1 because
// the trie reads it, numbers a letter's slot from zero, and this is that slot plus one. Only the
// problems read the numbering, so it is Domain's (ARCHITECTURE §17.6, §17.6a). How a strategy decodes
// or hashes with the numbers stays with it, and the baselines keep their own copies (§17.5).
/// <summary>The letters' numbers, A as 1 through Z as 26.</summary>
public static class LetterNumbers
{
    /// <summary>The last letter's number: Z is 26.</summary>
    public const int Last = LowercaseAlphabet.Size;

    /// <summary>A lowercase letter's number, 'a' as 1 through 'z' as 26.</summary>
    /// <param name="letter">A lowercase English letter, 'a' through 'z'.</param>
    /// <returns>The letter's number.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is outside 'a' through 'z'.</exception>
    public static int NumberOfLowercase(char letter) => LowercaseAlphabet.IndexOf(letter) + 1;
}
