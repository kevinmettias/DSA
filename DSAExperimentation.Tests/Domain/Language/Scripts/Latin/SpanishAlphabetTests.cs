using DSAExperimentation.Domain.Language.Scripts.Latin;

namespace DSAExperimentation.Tests.Domain.Language.Scripts.Latin;

public sealed partial class SpanishAlphabetTests
{
    [Fact]
    public void AccentedLetters_AreTheFiveAcuteVowelsUmlautedUAndEnye() =>
        Assert.Equal("áéíóúüñ", new string(SpanishAlphabet.AccentedLetters.AsSpan()));

    [Fact]
    public void AccentedLetters_EachIsABasicLetterWithOneMark() =>
        Assert.All(SpanishAlphabet.AccentedLetters, letter => Assert.Single(LatinScript.Decompose(letter).Diacritics));

    [Fact]
    public void OtherLetters_AreNone() => Assert.Empty(SpanishAlphabet.OtherLetters);
}
