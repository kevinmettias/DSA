using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.Domain.Language.English;

public sealed partial class EnglishConsonantsTests
{
    [Fact]
    public void Letters_AreTheTwentyOneConsonants() =>
        Assert.Equal("bcdfghjklmnpqrstvwxyz", new string(EnglishConsonants.Letters.AsSpan()));

    [Fact]
    public void Count_IsTwentyOne() => Assert.Equal(21, EnglishConsonants.Count);

    // Every char: an English letter, in either case, is a consonant exactly when it is no vowel, and
    // anything else is neither.
    [Fact]
    public void Has_IsEveryEnglishLetterThatIsNoVowelForEveryChar() =>
        Assert.All(
            Enumerable.Range(char.MinValue, char.MaxValue + 1).Select(code => (char)code),
            letter => Assert.Equal(IsEnglishLetter(letter) && !EnglishVowels.Has(letter), EnglishConsonants.Has(letter)));

    private static bool IsEnglishLetter(char letter) => letter is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
}
