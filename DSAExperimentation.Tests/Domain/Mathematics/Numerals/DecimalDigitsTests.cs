using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.Domain.Mathematics.Numerals;

// A digit's slot is its value. The characters on either side of '0'..'9' - '/' and ':' - are the
// ones an off-by-one in the slot arithmetic would let through, so they are the cases that must throw.
public sealed partial class DecimalDigitsTests
{
    public static TheoryData<char, int> DigitsToValues =>
        new() { { '0', 0 }, { '7', 7 }, { '9', 9 } };

    public static TheoryData<char> NonDigits =>
        new() { '/', ':', 'a' };

    public static TheoryData<int> NonValues =>
        new() { -1, 10 };

    [Fact]
    public void Size_IsTen() => Assert.Equal(10, DecimalDigits.Size);

    [Theory]
    [MemberData(nameof(DigitsToValues))]
    public void IndexOf_Digit_ReturnsItsValue(char digit, int expected) =>
        Assert.Equal(expected, DecimalDigits.IndexOf(digit));

    [Theory]
    [MemberData(nameof(NonDigits))]
    public void IndexOf_NotADigit_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DecimalDigits.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonDigits))]
    public void TryIndexOf_NotADigit_IsFalse(char letter) => Assert.False(DecimalDigits.TryIndexOf(letter, out _));

    [Theory]
    [MemberData(nameof(NonValues))]
    public void LetterAt_OutsideTheValues_Throws(int index) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DecimalDigits.LetterAt(index));

    [Fact]
    public void LetterAt_IndexOf_RoundTripsEveryValue() =>
        Assert.All(Enumerable.Range(0, DecimalDigits.Size), index =>
            Assert.Equal(index, DecimalDigits.IndexOf(DecimalDigits.LetterAt(index))));
}
