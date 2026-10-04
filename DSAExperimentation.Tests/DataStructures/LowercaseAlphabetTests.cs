using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Tests.DataStructures;

// The characters on either side of 'a'..'z' - '`' and '{' - are the ones an off-by-one in the slot
// arithmetic would let through, so they are the cases that must throw.
public sealed partial class LowercaseAlphabetTests
{
    public static TheoryData<char, int> LettersToSlots =>
        new() { { 'a', 0 }, { 'm', 12 }, { 'z', 25 } };

    public static TheoryData<char> NonLowercaseLetters =>
        new() { '`', '{', 'A', '0' };

    public static TheoryData<int, char> SlotsToLetters =>
        new() { { 0, 'a' }, { 25, 'z' } };

    public static TheoryData<int> NonSlots =>
        new() { -1, 26 };

    [Fact]
    public void Size_IsTwentySix() =>
        Assert.Equal(26, LowercaseAlphabet.Size);

    [Theory]
    [MemberData(nameof(LettersToSlots))]
    public void IndexOf_LowercaseLetter_ReturnsItsSlot(char letter, int expected) =>
        Assert.Equal(expected, LowercaseAlphabet.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLowercaseLetters))]
    public void IndexOf_NotALowercaseLetter_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => LowercaseAlphabet.IndexOf(letter));

    [Theory]
    [MemberData(nameof(SlotsToLetters))]
    public void LetterAt_Slot_ReturnsItsLetter(int index, char expected) =>
        Assert.Equal(expected, LowercaseAlphabet.LetterAt(index));

    [Theory]
    [MemberData(nameof(NonSlots))]
    public void LetterAt_OutsideTheSlots_Throws(int index) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => LowercaseAlphabet.LetterAt(index));

    [Fact]
    public void LetterAt_IndexOf_RoundTripsEverySlot() =>
        Assert.All(Enumerable.Range(0, LowercaseAlphabet.Size), index =>
            Assert.Equal(index, LowercaseAlphabet.IndexOf(LowercaseAlphabet.LetterAt(index))));
}
