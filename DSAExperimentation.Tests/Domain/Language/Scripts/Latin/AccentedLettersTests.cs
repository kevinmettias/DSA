using DSAExperimentation.Domain.Language.Scripts.Latin;

namespace DSAExperimentation.Tests.Domain.Language.Scripts.Latin;

// Read through Spanish's alphabet, and French's for folding a word: an accented letter of the
// alphabet is found in either case and folds to its basic letter in the same case, while a letter
// the alphabet does not write, an umlaut in Spanish or ø anywhere, is left as it is.
public sealed partial class AccentedLettersTests
{
    public static TheoryData<char> SpanishAccents => new() { 'ñ', 'Ñ', 'á', 'Ú' };

    public static TheoryData<char> NoSpanishAccents => new() { 'n', 'ä', 'ß' };

    public static TheoryData<char, char> SpanishBases => new()
    {
        { 'ñ', 'n' },
        { 'Ñ', 'N' },
        { 'ü', 'u' },
        { 'É', 'E' },
        { 'x', 'x' },
        { 'ä', 'ä' },
        { 'ø', 'ø' },
    };

    [Theory]
    [MemberData(nameof(SpanishAccents))]
    public void IsAccented_AccentedLetterInEitherCase_IsTrue(char letter) =>
        Assert.True(AccentedLetters<SpanishAlphabet>.IsAccented(letter));

    [Theory]
    [MemberData(nameof(NoSpanishAccents))]
    public void IsAccented_LetterTheAlphabetDoesNotAccent_IsFalse(char letter) =>
        Assert.False(AccentedLetters<SpanishAlphabet>.IsAccented(letter));

    [Theory]
    [MemberData(nameof(SpanishBases))]
    public void BaseOf_Letter_FoldsTheAlphabetsAccentedLettersAlone(char letter, char expected) =>
        Assert.Equal(expected, AccentedLetters<SpanishAlphabet>.BaseOf(letter));

    [Fact]
    public void BaseOf_EachLetterOfAWord_SetsItsAccentsAside() =>
        Assert.Equal("cafe naif", string.Concat("café naïf".Select(AccentedLetters<FrenchAlphabet>.BaseOf)));

    [Fact]
    public void DiacriticsOf_AccentedLetter_IsItsMarks() =>
        Assert.Equal<Diacritic>([Diacritic.Tilde], AccentedLetters<SpanishAlphabet>.DiacriticsOf('Ñ'));

    [Fact]
    public void DiacriticsOf_LetterTheAlphabetDoesNotAccent_IsEmpty() =>
        Assert.Empty(AccentedLetters<GermanAlphabet>.DiacriticsOf('é'));
}
