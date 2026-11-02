using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.Domain.Language.English;

// The characters on either side of 'a'..'z' - '`' and '{' - are the ones an off-by-one in the slot
// arithmetic would let through, so they are the cases that must throw.
public sealed partial class EnglishLowercaseTests
{
    public static TheoryData<char, int> LettersToSlots =>
        new() { { 'a', 0 }, { 'm', 12 }, { 'z', 25 } };

    public static TheoryData<char> NonLowercaseLetters =>
        new() { '`', '{', 'A', '0' };

    public static TheoryData<int, char> SlotsToLetters =>
        new() { { 0, 'a' }, { 25, 'z' } };

    public static TheoryData<int> NonSlots =>
        new() { -1, 26 };

    [Theory]
    [MemberData(nameof(LettersToSlots))]
    public void IndexOf_LowercaseLetter_ReturnsItsSlot(char letter, int expected) =>
        Assert.Equal(expected, EnglishLowercase.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLowercaseLetters))]
    public void IndexOf_NotALowercaseLetter_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => EnglishLowercase.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLowercaseLetters))]
    public void TryIndexOf_NotALowercaseLetter_IsFalse(char letter) =>
        Assert.False(EnglishLowercase.TryIndexOf(letter, out _));

    [Theory]
    [MemberData(nameof(SlotsToLetters))]
    public void LetterAt_Slot_ReturnsItsLetter(int index, char expected) =>
        Assert.Equal(expected, EnglishLowercase.LetterAt(index));

    [Theory]
    [MemberData(nameof(NonSlots))]
    public void LetterAt_OutsideTheSlots_Throws(int index) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => EnglishLowercase.LetterAt(index));

    [Fact]
    public void LetterAt_IndexOf_RoundTripsEverySlot() =>
        Assert.All(Enumerable.Range(0, EnglishLetters.Size), index =>
            Assert.Equal(index, EnglishLowercase.IndexOf(EnglishLowercase.LetterAt(index))));
}
