using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.Domain.Language.English;

public sealed partial class EnglishVowelsTests
{
    [Fact]
    public void Count_IsTheNumberOfLetters() => Assert.Equal(EnglishVowels.Count, EnglishVowels.Letters.Length);

    [Fact]
    public void Letters_AreTheFiveVowels() => Assert.Equal("aeiou", EnglishVowels.Letters);

    // Every char, so a vowel the predicate misses, or a consonant or an uppercase vowel it accepts,
    // shows as a disagreement with the letters.
    [Fact]
    public void IsLowercaseVowel_AgreesWithLettersForEveryChar() =>
        Assert.All(
            Enumerable.Range(char.MinValue, char.MaxValue + 1).Select(code => (char)code),
            letter => Assert.Equal(EnglishVowels.Letters.Contains(letter), EnglishVowels.IsLowercaseVowel(letter)));
}
