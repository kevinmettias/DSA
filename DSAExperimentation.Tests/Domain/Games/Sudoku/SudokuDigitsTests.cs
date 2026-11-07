using DSAExperimentation.Domain.Games.Sudoku;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku;

// '0' and ':' sit either side of '1'..'9', so they are the cases an off-by-one would let through.
public sealed partial class SudokuDigitsTests
{
    public static TheoryData<char> NonDigits => new() { '0', ':', '.' };

    [Fact]
    public void Size_IsTheBoardsSide() => Assert.Equal(ClassicSudoku.Size, SudokuDigits.Size);

    [Fact]
    public void IndexOf_One_IsTheFirstSlot() => Assert.Equal(0, SudokuDigits.IndexOf('1'));

    [Theory]
    [MemberData(nameof(NonDigits))]
    public void TryIndexOf_NotASudokuDigit_IsFalse(char letter) => Assert.False(SudokuDigits.TryIndexOf(letter, out _));

    [Fact]
    public void LetterAt_LastSlot_IsNine() => Assert.Equal('9', SudokuDigits.LetterAt(8));

    // The two ends are constants; each must be the digit its slot holds.
    [Fact]
    public void FirstDigitAndLastDigit_AreTheDigitsInTheFirstAndLastSlots() =>
        Assert.Equal(
            (SudokuDigits.LetterAt(0), SudokuDigits.LetterAt(SudokuDigits.Size - 1)),
            (SudokuDigits.FirstDigit, SudokuDigits.LastDigit));
}
