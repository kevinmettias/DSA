using DSAExperimentation.Domain.Language.Scripts.Latin;

namespace DSAExperimentation.Tests.Domain.Language.Scripts.Latin;

public sealed partial class GermanAlphabetTests
{
    [Fact]
    public void AccentedLetters_AreTheUmlauts() => Assert.Equal("äöü", new string(GermanAlphabet.AccentedLetters.AsSpan()));

    // The umlaut is the diaeresis mark, one character in Unicode.
    [Fact]
    public void AccentedLetters_EachCarriesTheDiaeresis() =>
        Assert.All(GermanAlphabet.AccentedLetters, letter =>
            Assert.Equal<Diacritic>([Diacritic.Diaeresis], LatinScript.Decompose(letter).Diacritics));

    [Fact]
    public void OtherLetters_IsTheSharpS() => Assert.Equal("ß", new string(GermanAlphabet.OtherLetters.AsSpan()));

    [Fact]
    public void OtherLetters_NoneDecomposes() =>
        Assert.All(GermanAlphabet.OtherLetters, letter => Assert.False(LatinScript.TryDecompose(letter, out _)));
}
