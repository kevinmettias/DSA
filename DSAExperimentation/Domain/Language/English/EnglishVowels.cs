using System.Collections.Immutable;
using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Language.English;

// English's five written vowels, a, e, i, o and u, as LC 1641, LC 2063 and LC 3227 state them: a
// letter set over the English letters with case set aside, so 'A' is a vowel as 'a' is. Y, a vowel
// in some words and a consonant in others, is not a written vowel of the alphabet and is not here.
// The letters are an array, the one source the count and the spelled-out string are read from; they
// are lowercase, the form LetterAt gives, so a set built from them holds five. These are the
// problems' *what* (ARCHITECTURE §17.6); how a solution counts or finds them stays with each
// strategy, and the baselines keep their own copies (§17.5).
/// <summary>The five English vowels, a, e, i, o and u, in either case.</summary>
public readonly struct EnglishVowels : ILetterSet<EnglishLetters>
{
    /// <summary>The vowels, lowercase, in alphabetical order.</summary>
    public static ImmutableArray<char> Letters { get; } = ['a', 'e', 'i', 'o', 'u'];

    /// <summary>The number of vowels, read from <see cref="Letters"/> once.</summary>
    public static int Count { get; } = Letters.Length;

    /// <summary>Spells the vowels out as one string.</summary>
    /// <returns>The vowels in alphabetical order, "aeiou".</returns>
    public static string LettersAsString() => new(Letters.AsSpan());

    /// <summary>Tests whether a character is a vowel, in either case.</summary>
    /// <param name="letter">The character to test.</param>
    /// <returns>True for a, e, i, o or u, lowercase or capital; otherwise false.</returns>
    public static bool Has(char letter) => LetterSet<EnglishVowels, EnglishLetters>.Has(letter);
}
