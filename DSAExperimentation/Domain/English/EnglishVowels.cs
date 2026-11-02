namespace DSAExperimentation.Domain.English;

// English's five vowels as LC 1641, LC 2063 and LC 3227 state them: a, e, i, o and u, in lowercase,
// since all three problems' strings are lowercase. These are the problems' *what* (ARCHITECTURE
// §17.6); how a solution counts or finds them stays with each strategy, and the baselines keep their
// own copies (§17.5). The predicate says lowercase because it means it: an uppercase vowel is not one
// of these, and a mixed-case caller would otherwise get a silent false.
/// <summary>The five lowercase English vowels.</summary>
public static class EnglishVowels
{
    /// <summary>The vowels, in alphabetical order.</summary>
    public const string Letters = "aeiou";

    /// <summary>The number of vowels, the length of <see cref="Letters"/>.</summary>
    public const int Count = 5;

    /// <summary>Whether a character is one of the five lowercase vowels.</summary>
    /// <param name="letter">The character to test.</param>
    /// <returns><see langword="true"/> for a, e, i, o or u; otherwise <see langword="false"/>.</returns>
    public static bool IsLowercaseVowel(char letter) => letter is 'a' or 'e' or 'i' or 'o' or 'u';
}
