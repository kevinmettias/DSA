using DSAExperimentation.Domain.Games.Sudoku;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku;

public sealed partial class SudokuTests
{
    public static TheoryData<int, int, int> CellsToBoxNumbers =>
        new() { { 0, 0, 0 }, { 0, 8, 2 }, { 2, 3, 1 }, { 4, 4, 4 }, { 5, 6, 5 }, { 8, 0, 6 }, { 8, 8, 8 } };

    // Against a board whose only digit is a 5 at the top-left corner: each blocked case shares
    // exactly one unit with it - its row, its column, its box - so each pins one of the checks.
    public static TheoryData<int, int, char> PlacementsATopLeftFiveBlocks =>
        new() { { 0, 5, '5' }, { 5, 0, '5' }, { 2, 2, '5' } };

    public static TheoryData<int, int, char> PlacementsATopLeftFiveAllows =>
        new() { { 4, 4, '5' }, { 0, 5, '6' } };

    [Theory]
    [MemberData(nameof(CellsToBoxNumbers))]
    public void BoxOf_NumbersBoxesRowByRow(int row, int col, int expected) =>
        Assert.Equal(expected, Sudoku<ClassicSudoku>.BoxOf(row, col));

    [Theory]
    [MemberData(nameof(PlacementsATopLeftFiveBlocks))]
    public void CanPlace_DigitItsRowColumnOrBoxHolds_ReturnsFalse(int row, int col, char digit) =>
        Assert.False(Sudoku<ClassicSudoku>.CanPlace(BoardWith((0, 0, '5')), row, col, digit));

    [Theory]
    [MemberData(nameof(PlacementsATopLeftFiveAllows))]
    public void CanPlace_DigitNoUnitHolds_ReturnsTrue(int row, int col, char digit) =>
        Assert.True(Sudoku<ClassicSudoku>.CanPlace(BoardWith((0, 0, '5')), row, col, digit));

    [Fact]
    public void IsComplete_EveryCellFilled_ReturnsTrue() => Assert.True(Sudoku<ClassicSudoku>.IsComplete(FilledBoard()));

    [Fact]
    public void IsComplete_OneEmptyCell_ReturnsFalse()
    {
        var board = FilledBoard();
        board[8][8] = ClassicSudoku.EmptyCell;

        Assert.False(Sudoku<ClassicSudoku>.IsComplete(board));
    }

    [Fact]
    public void IsComplete_EmptyBoard_ReturnsFalse() => Assert.False(Sudoku<ClassicSudoku>.IsComplete(BoardWith()));

    // A 4x4 board of 2x2 boxes: the same rule on a grid of order 2.
    [Fact]
    public void Size_OrderTwoGrid_IsFour() => Assert.Equal(4, Sudoku<OrderTwo>.Size);

    [Fact]
    public void BoxOf_OrderTwoGrid_NumbersItsFourBoxes() => Assert.Equal(3, Sudoku<OrderTwo>.BoxOf(3, 2));

    [Fact]
    public void CanPlace_OrderTwoGrid_ReadsItsTwoByTwoBox()
    {
        char[][] board = [['1', '.', '.', '.'], ['.', '.', '.', '.'], ['.', '.', '.', '.'], ['.', '.', '.', '.']];

        Assert.False(Sudoku<OrderTwo>.CanPlace(board, 1, 1, '1'));
        Assert.True(Sudoku<OrderTwo>.CanPlace(board, 2, 2, '1'));
    }

    private static char[][] BoardWith(params (int Row, int Col, char Digit)[] cells)
    {
        var board = Enumerable.Range(0, ClassicSudoku.Size)
            .Select(_ => Enumerable.Repeat(ClassicSudoku.EmptyCell, ClassicSudoku.Size).ToArray())
            .ToArray();

        foreach (var (row, col, digit) in cells)
        {
            board[row][col] = digit;
        }

        return board;
    }

    private static char[][] FilledBoard() =>
        Enumerable.Range(0, ClassicSudoku.Size)
            .Select(_ => Enumerable.Repeat(SudokuDigits.LetterAt(0), ClassicSudoku.Size).ToArray())
            .ToArray();

    private readonly struct OrderTwo : ISudokuGrid
    {
        public static int BoxSize => 2;

        public static char EmptyCell => '.';
    }
}
