using DSAExperimentation.DataStructures.Alphabets;
using DSAExperimentation.Tests.DataStructures.Alphabets.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Alphabets;

// Russian's lowercase letters, laid out by a table: е is the sixth letter, ё the seventh though it
// is coded after я, and ж the eighth. ѐ (U+0450) sits between я and ё, a gap inside the table's
// range, and Ђ (U+0402) lies below а, outside it; both must be refused.
public sealed partial class LetterTableTests
{
    public static TheoryData<char, int> LettersToSlots =>
        new() { { 'а', 0 }, { 'е', 5 }, { 'ё', 6 }, { 'ж', 7 }, { 'я', 32 } };

    public static TheoryData<char> NonLetters =>
        new() { '\u0450', '\u0402', 'a', 'Ё' };

    public static TheoryData<int> NonSlots =>
        new() { -1, 33 };

    [Fact]
    public void Size_IsTheNumberOfLetters() => Assert.Equal(33, RussianLowercase.Size);

    [Fact]
    public void Letters_AreTheLettersInSlotOrder() =>
        Assert.Equal<char>(['д', 'е', 'ё', 'ж'], new LetterTable([.. "абвгдеёж"]).Letters[4..]);

    [Fact]
    public void LetterTable_RepeatedLetter_Throws() =>
        Assert.Throws<ArgumentException>(() => new LetterTable([.. "abca"]));

    [Theory]
    [MemberData(nameof(LettersToSlots))]
    public void IndexOf_Letter_ReturnsItsSlot(char letter, int expected) =>
        Assert.Equal(expected, RussianLowercase.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void IndexOf_NotALetter_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RussianLowercase.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void TryIndexOf_NotALetter_IsFalse(char letter) => Assert.False(RussianLowercase.TryIndexOf(letter, out _));

    [Theory]
    [MemberData(nameof(NonSlots))]
    public void LetterAt_OutsideTheSlots_Throws(int index) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RussianLowercase.LetterAt(index));

    [Fact]
    public void LetterAt_IndexOf_RoundTripsEverySlot() =>
        Assert.All(Enumerable.Range(0, RussianLowercase.Size), index =>
            Assert.Equal(index, RussianLowercase.IndexOf(RussianLowercase.LetterAt(index))));
}
