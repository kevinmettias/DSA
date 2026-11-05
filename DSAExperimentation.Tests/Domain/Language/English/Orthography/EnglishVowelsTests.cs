using DSAExperimentation.Domain.Language.English.Orthography;

namespace DSAExperimentation.Tests.Domain.Language.English.Orthography;

public sealed partial class EnglishVowelsTests
{
    [Fact]
    public void Letters_AreTheFiveVowels() => Assert.Equal<char>(['a', 'e', 'i', 'o', 'u'], EnglishVowels.Letters);

    [Fact]
    public void Count_IsFive() => Assert.Equal(5, EnglishVowels.Count);

    [Fact]
    public void LettersAsString_SpellsTheLettersInOrder() => Assert.Equal("aeiou", EnglishVowels.LettersAsString());

    // Every char, so a vowel the test misses in either case, or a consonant, a 'y' or a non-English
    // character it accepts, shows as a disagreement with the letters.
    [Fact]
    public void Has_AgreesWithLettersInEitherCaseForEveryChar() =>
        Assert.All(
            Enumerable.Range(char.MinValue, char.MaxValue + 1).Select(code => (char)code),
            letter => Assert.Equal(IsListedVowel(letter), EnglishVowels.Has(letter)));

    private static bool IsListedVowel(char letter) =>
        letter is >= 'A' and <= 'Z' or >= 'a' and <= 'z' && EnglishVowels.Letters.Contains(char.ToLowerInvariant(letter));
}
