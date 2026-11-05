using System.Collections.Immutable;
using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Language.English;

// English's 21 written consonants: every letter of the alphabet that is not one of its five written
// vowels. The set is read from EnglishVowels rather than listed again, so the two split the alphabet
// and a letter is in exactly one. Y is here, since it is not a written vowel; that it spells a vowel
// sound in some words is a fact of English's sounds, not of its alphabet. Case is set aside as for
// the vowels, and the letters are lowercase, in alphabetical order. Membership is LetterSet's mask,
// built once, as the vowels' is.
/// <summary>The 21 English consonants, every letter but a, e, i, o and u, in either case.</summary>
public readonly struct EnglishConsonants : ILetterSet<EnglishLetters>
{
    /// <summary>The consonants, lowercase, in alphabetical order.</summary>
    public static ImmutableArray<char> Letters { get; } =
        [.. EnglishLowercase.Letters.Where(letter => !EnglishVowels.Letters.Contains(letter))];

    /// <summary>The number of consonants, read from <see cref="Letters"/> once.</summary>
    public static int Count { get; } = Letters.Length;

    /// <summary>Tests whether a character is a consonant, in either case.</summary>
    /// <param name="letter">The character to test.</param>
    /// <returns>True for an English letter other than a, e, i, o or u, lowercase or capital; otherwise false.</returns>
    public static bool Has(char letter) => LetterSet<EnglishConsonants, EnglishLetters>.Has(letter);
}
