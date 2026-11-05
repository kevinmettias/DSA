using DSAExperimentation.Domain.Language.Scripts.Latin;

namespace DSAExperimentation.Tests.Domain.Language.Scripts.Latin;

public sealed partial class FrenchAlphabetTests
{
    [Fact]
    public void AccentedLetters_AreTheVowelsWithTheirAccentsAndCWithItsCedilla() =>
        Assert.Equal("àâçéèêëîïôùûüÿ", new string(FrenchAlphabet.AccentedLetters.AsSpan()));

    [Fact]
    public void AccentedLetters_EachIsABasicLetterWithOneMark() =>
        Assert.All(FrenchAlphabet.AccentedLetters, letter => Assert.Single(LatinScript.Decompose(letter).Diacritics));

    [Fact]
    public void OtherLetters_AreTheLigatures() => Assert.Equal("æœ", new string(FrenchAlphabet.OtherLetters.AsSpan()));

    [Fact]
    public void OtherLetters_NoneDecomposes() =>
        Assert.All(FrenchAlphabet.OtherLetters, letter => Assert.False(LatinScript.TryDecompose(letter, out _)));
}
