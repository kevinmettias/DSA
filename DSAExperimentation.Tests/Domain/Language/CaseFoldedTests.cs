using System.Globalization;
using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.Tests.DataStructures.Alphabets.Fixtures;

namespace DSAExperimentation.Tests.Domain.Language;

// English's pairing, the one CaseFolded is used with. A letter and its capital share a slot, and a
// character that is a letter in neither case is refused.
public sealed partial class CaseFoldedTests
{
    public static TheoryData<char, int> LettersToSlots =>
        new() { { 'a', 0 }, { 'A', 0 }, { 'm', 12 }, { 'M', 12 }, { 'z', 25 }, { 'Z', 25 } };

    public static TheoryData<char> NonLetters => new() { '@', '[', '`', '{', '1' };

    [Fact]
    public void Size_IsTheNumberOfLetters() =>
        Assert.Equal(EnglishLetters.Size, CaseFolded<EnglishLowercase, EnglishUppercase>.Size);

    [Theory]
    [MemberData(nameof(LettersToSlots))]
    public void IndexOf_LetterInEitherCase_ReturnsTheSharedSlot(char letter, int expected) =>
        Assert.Equal(expected, CaseFolded<EnglishLowercase, EnglishUppercase>.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void IndexOf_LetterInNeitherCase_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => CaseFolded<EnglishLowercase, EnglishUppercase>.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void TryIndexOf_LetterInNeitherCase_IsFalse(char letter) =>
        Assert.False(CaseFolded<EnglishLowercase, EnglishUppercase>.TryIndexOf(letter, out _));

    // Russian pairs its cases letter for letter too, ё with Ё, though neither is coded beside its
    // neighbours: each capital takes its lowercase letter's slot.
    [Fact]
    public void IndexOf_RussianCapital_TakesItsLowercaseLettersSlot() =>
        Assert.All(Enumerable.Range(0, RussianLowercase.Size), index =>
        {
            var capital = char.ToUpper(RussianLowercase.LetterAt(index), CultureInfo.InvariantCulture);

            Assert.Equal(index, CaseFolded<RussianLowercase, RussianUppercase>.IndexOf(capital));
        });

    [Fact]
    public void LetterAt_Slot_ReturnsTheLowercaseForm() =>
        Assert.Equal('q', CaseFolded<EnglishLowercase, EnglishUppercase>.LetterAt(EnglishUppercase.IndexOf('Q')));
}
