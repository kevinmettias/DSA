using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Tests.DataStructures.Alphabets;

// A run of the ten digits. The characters on either side of '0'..'9' - '/' and ':' - are the ones
// an off-by-one in the slot arithmetic would let through, so they are the cases that must fail.
// IndexOf finds a letter's slot through TryIndexOf, so its cases cover TryIndexOf's success too.
public sealed partial class LetterRunTests
{
    private static readonly LetterRun Digits = new('0', 10);

    public static TheoryData<char, int> LettersToSlots =>
        new() { { '0', 0 }, { '4', 4 }, { '9', 9 } };

    public static TheoryData<char> NonLetters =>
        new() { '/', ':', 'a' };

    public static TheoryData<int, char> SlotsToLetters =>
        new() { { 0, '0' }, { 9, '9' } };

    public static TheoryData<int> NonSlots =>
        new() { -1, 10 };

    [Theory]
    [MemberData(nameof(LettersToSlots))]
    public void IndexOf_Letter_ReturnsItsDistanceFromTheFirst(char letter, int expected) =>
        Assert.Equal(expected, Digits.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void IndexOf_OutsideTheRun_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Digits.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void TryIndexOf_OutsideTheRun_IsFalse(char letter) => Assert.False(Digits.TryIndexOf(letter, out _));

    [Theory]
    [MemberData(nameof(SlotsToLetters))]
    public void LetterAt_Slot_ReturnsItsLetter(int index, char expected) =>
        Assert.Equal(expected, Digits.LetterAt(index));

    [Theory]
    [MemberData(nameof(NonSlots))]
    public void LetterAt_OutsideTheSlots_Throws(int index) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Digits.LetterAt(index));

    [Fact]
    public void LetterAt_IndexOf_RoundTripsEverySlot() =>
        Assert.All(Enumerable.Range(0, Digits.Size), index =>
            Assert.Equal(index, Digits.IndexOf(Digits.LetterAt(index))));
}
