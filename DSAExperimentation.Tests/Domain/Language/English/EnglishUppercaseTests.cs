using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.Domain.Language.English;

// The characters on either side of 'A'..'Z' - '@' and '[' - are the ones an off-by-one in the slot
// arithmetic would let through, so they are the cases that must throw.
public sealed partial class EnglishUppercaseTests
{
    public static TheoryData<char, int> LettersToSlots =>
        new() { { 'A', 0 }, { 'M', 12 }, { 'Z', 25 } };

    public static TheoryData<char> NonUppercaseLetters =>
        new() { '@', '[', 'a', '0' };

    public static TheoryData<int, char> SlotsToLetters =>
        new() { { 0, 'A' }, { 25, 'Z' } };

    public static TheoryData<int> NonSlots =>
        new() { -1, 26 };

    [Theory]
    [MemberData(nameof(LettersToSlots))]
    public void IndexOf_UppercaseLetter_ReturnsItsSlot(char letter, int expected) =>
        Assert.Equal(expected, EnglishUppercase.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonUppercaseLetters))]
    public void IndexOf_NotAnUppercaseLetter_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => EnglishUppercase.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonUppercaseLetters))]
    public void TryIndexOf_NotAnUppercaseLetter_IsFalse(char letter) =>
        Assert.False(EnglishUppercase.TryIndexOf(letter, out _));

    [Theory]
    [MemberData(nameof(SlotsToLetters))]
    public void LetterAt_Slot_ReturnsItsLetter(int index, char expected) =>
        Assert.Equal(expected, EnglishUppercase.LetterAt(index));

    [Theory]
    [MemberData(nameof(NonSlots))]
    public void LetterAt_OutsideTheSlots_Throws(int index) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => EnglishUppercase.LetterAt(index));

    [Fact]
    public void LetterAt_IndexOf_RoundTripsEverySlot() =>
        Assert.All(Enumerable.Range(0, EnglishLetters.Size), index =>
            Assert.Equal(index, EnglishUppercase.IndexOf(EnglishUppercase.LetterAt(index))));

    // The two ends are constants; each must be the letter its slot holds.
    [Fact]
    public void FirstLetterAndLastLetter_AreTheLettersInTheFirstAndLastSlots() =>
        Assert.Equal(
            (EnglishUppercase.LetterAt(0), EnglishUppercase.LetterAt(EnglishLetters.Size - 1)),
            (EnglishUppercase.FirstLetter, EnglishUppercase.LastLetter));
}
