using System.Globalization;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.Domain.Language.English;

public sealed partial class EnglishLettersTests
{
    public static TheoryData<char, int> LettersToNumbers => new()
    {
        { 'a', 1 },
        { 'A', 1 },
        { 'j', 10 },
        { 'y', 25 },
        { 'z', 26 },
        { 'Z', 26 },
    };

    public static TheoryData<char> NonLetters => new() { '@', '[', '`', '{', '1', 'é' };

    [Fact]
    public void Size_IsTwentySix() => Assert.Equal(26, EnglishLetters.Size);

    [Fact]
    public void Size_IsTheNumberOfZ() => Assert.Equal(EnglishLetters.Size, EnglishLetters.NumberOf('z'));

    // English pairs its cases one for one: slot i holds a letter and its own capital.
    [Fact]
    public void IndexOf_EverySlot_PairsALetterWithItsCapital() =>
        Assert.All(Enumerable.Range(0, EnglishLetters.Size), index =>
        {
            var lower = EnglishLowercase.LetterAt(index);
            var upper = EnglishUppercase.LetterAt(index);

            Assert.Equal(char.ToUpper(lower, CultureInfo.InvariantCulture), upper);
            Assert.Equal(index, EnglishLetters.IndexOf(upper));
        });

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void IndexOf_NotAnEnglishLetter_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => EnglishLetters.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void TryIndexOf_NotAnEnglishLetter_IsFalse(char letter) =>
        Assert.False(EnglishLetters.TryIndexOf(letter, out _));

    [Fact]
    public void LetterAt_Slot_ReturnsTheLowercaseLetter() => Assert.Equal('c', EnglishLetters.LetterAt(2));

    [Theory]
    [MemberData(nameof(LettersToNumbers))]
    public void NumberOf_Letter_CountsFromOneAtA(char letter, int number) =>
        Assert.Equal(number, EnglishLetters.NumberOf(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void NumberOf_NotAnEnglishLetter_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => EnglishLetters.NumberOf(letter));
}
