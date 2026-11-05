using System.Globalization;
using System.Text;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.Domain.Language.Scripts.Latin;

namespace DSAExperimentation.Tests.Domain.Language.Scripts.Latin;

// Each named diacritic is checked by a letter that carries it, taken apart through Unicode, so a
// mark written with the wrong code point decomposes to the wrong name. The letters that are no
// basic letter with marks are refused, and every char that does decompose composes back to itself.
public sealed partial class LatinScriptTests
{
    public static TheoryData<char, char, string> AccentedLetters => new()
    {
        { 'à', 'a', "grave" },
        { 'é', 'e', "acute" },
        { 'É', 'E', "acute" },
        { 'ê', 'e', "circumflex" },
        { 'ñ', 'n', "tilde" },
        { 'ā', 'a', "macron" },
        { 'ă', 'a', "breve" },
        { 'ż', 'z', "dot above" },
        { 'ä', 'a', "diaeresis" },
        { 'ë', 'e', "diaeresis" },
        { 'ả', 'a', "hook above" },
        { 'å', 'a', "ring above" },
        { 'ő', 'o', "double acute" },
        { 'č', 'c', "caron" },
        { 'ơ', 'o', "horn" },
        { 'ạ', 'a', "dot below" },
        { 'ș', 's', "comma below" },
        { 'ç', 'c', "cedilla" },
        { 'ą', 'a', "ogonek" },
        { 'ế', 'e', "circumflex, acute" },
        { 'ǖ', 'u', "diaeresis, macron" },
        { 'e', 'e', "" },
    };

    // Letters with a stroke, letters of their own, a letter built on one of them, another script's
    // accented letter, and characters that are no letters at all, half a surrogate pair among them.
    public static TheoryData<char> NotDecomposable => new() { 'ø', 'ł', 'ß', 'æ', 'œ', 'ǿ', 'ά', '1', ' ', (char)0xD800 };

    [Fact]
    public void Diacritics_AreSeventeen() => Assert.Equal(17, LatinScript.Diacritics.Length);

    [Fact]
    public void Diacritics_AreInTheOrderOfTheirMarks() =>
        Assert.Equal(LatinScript.Diacritics.OrderBy(diacritic => diacritic.CombiningMark), LatinScript.Diacritics);

    [Fact]
    public void Diacritics_EachIsACombiningMark() =>
        Assert.All(LatinScript.Diacritics, diacritic =>
            Assert.Equal(UnicodeCategory.NonSpacingMark, CharUnicodeInfo.GetUnicodeCategory(diacritic.CombiningMark)));

    [Fact]
    public void Diacritics_EachHasItsOwnName() =>
        Assert.Equal(LatinScript.Diacritics.Length, LatinScript.Diacritics.Select(diacritic => diacritic.Name).Distinct().Count());

    [Fact]
    public void IsBasicLetter_EveryChar_IsTheTwentySixLettersInEitherCase() =>
        Assert.All(AllChars(), letter =>
            Assert.Equal(letter is >= 'a' and <= 'z' or >= 'A' and <= 'Z', LatinScript.IsBasicLetter(letter)));

    [Theory]
    [MemberData(nameof(AccentedLetters))]
    public void Decompose_Letter_IsItsBasicLetterAndMarks(char letter, char baseLetter, string marks) =>
        Assert.Equal((baseLetter, marks), Spelled(LatinScript.Decompose(letter)));

    // English writes the basic letters with nothing on them, so its alphabet needs no decomposing.
    [Fact]
    public void Decompose_EveryEnglishLetter_CarriesNoMark() =>
        Assert.All(EnglishLowercase.Letters, letter => Assert.Empty(LatinScript.Decompose(letter).Diacritics));

    [Fact]
    public void Decompose_LetterWithAStroke_Throws() =>
        Assert.Throws<ArgumentException>(() => LatinScript.Decompose('ø'));

    [Theory]
    [MemberData(nameof(NotDecomposable))]
    public void TryDecompose_NoBasicLetterWithNamedMarks_IsFalse(char letter) =>
        Assert.False(LatinScript.TryDecompose(letter, out _));

    [Fact]
    public void TryDecompose_EveryCharItTakesApart_ComposesBackToIt() =>
        Assert.All(AllChars().Where(letter => LatinScript.TryDecompose(letter, out _)), letter =>
            Assert.Equal(letter.ToString().Normalize(NormalizationForm.FormC), Recomposed(LatinScript.Decompose(letter))));

    private static IEnumerable<char> AllChars() =>
        Enumerable.Range(char.MinValue, char.MaxValue + 1).Select(code => (char)code);

    private static (char BaseLetter, string Marks) Spelled(DecomposedLetter decomposed) =>
        (decomposed.BaseLetter, string.Join(", ", decomposed.Diacritics.Select(diacritic => diacritic.Name)));

    private static string Recomposed(DecomposedLetter decomposed) =>
        string.Concat(decomposed.Diacritics.Select(diacritic => diacritic.CombiningMark).Prepend(decomposed.BaseLetter))
            .Normalize(NormalizationForm.FormC);
}
