using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.Domain.Mathematics.Numerals;

// A digit's slot is its value. The characters on either side of '0'..'1' - '/' and '2' - are the
// ones an off-by-one in the slot arithmetic would let through, so they are the cases that must
// throw.
public sealed partial class BinaryDigitsTests
{
    public static TheoryData<char, int> DigitsToValues =>
        new() { { '0', 0 }, { '1', 1 } };

    public static TheoryData<char> NonDigits =>
        new() { '/', '2', 'b' };

    public static TheoryData<int> NonValues =>
        new() { -1, 2 };

    [Fact]
    public void Size_IsTwo() => Assert.Equal(2, BinaryDigits.Size);

    [Theory]
    [MemberData(nameof(DigitsToValues))]
    public void IndexOf_Digit_ReturnsItsValue(char digit, int expected) =>
        Assert.Equal(expected, BinaryDigits.IndexOf(digit));

    [Theory]
    [MemberData(nameof(NonDigits))]
    public void IndexOf_NotADigit_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => BinaryDigits.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonDigits))]
    public void TryIndexOf_NotADigit_IsFalse(char letter) => Assert.False(BinaryDigits.TryIndexOf(letter, out _));

    [Theory]
    [MemberData(nameof(NonValues))]
    public void LetterAt_OutsideTheValues_Throws(int index) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => BinaryDigits.LetterAt(index));

    [Fact]
    public void LetterAt_IndexOf_RoundTripsEveryValue() =>
        Assert.All(Enumerable.Range(0, BinaryDigits.Size), index =>
            Assert.Equal(index, BinaryDigits.IndexOf(BinaryDigits.LetterAt(index))));
}
