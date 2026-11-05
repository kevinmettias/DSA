using DSAExperimentation.Domain.Games.Sudoku;
using DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku;

// The solved boards are the fact itself, one per grid: LC 37's example solved, a 6x6 board of 2x3
// boxes, a 16x16 board written in 0 to F, the 4x4 children's grid and the 5x5 jigsaw. Each unsolved
// board fails one clause: a cell left empty, a column repeat, or a Latin square whose regions
// repeat.
public sealed partial class SudokuTests
{
    public static TheoryData<int, int, int> CellsToBoxNumbers => new()
    {
        { 0, 0, 0 }, { 0, 8, 2 }, { 2, 3, 1 }, { 4, 4, 4 }, { 5, 6, 5 }, { 8, 0, 6 }, { 8, 8, 8 },
    };

    // Against a board whose only digit is a 5 at the top-left corner: each blocked case shares
    // exactly one unit with it - its row, its column, its box - so each pins one of the checks.
    public static TheoryData<int, int, char> PlacementsATopLeftFiveBlocks =>
        new() { { 0, 5, '5' }, { 5, 0, '5' }, { 2, 2, '5' } };

    public static TheoryData<int, int, char> PlacementsATopLeftFiveAllows =>
        new() { { 4, 4, '5' }, { 0, 5, '6' } };

    public static TheoryData<string, string[]> SolvedBoards => new()
    {
        {
            nameof(ClassicSudoku),
            [
                "534678912", "672195348", "198342567", "859761423", "426853791",
                "713924856", "961537284", "287419635", "345286179",
            ]
        },
        { nameof(SixBySixSudoku), ["123456", "456123", "234561", "561234", "345612", "612345"] },
        {
            nameof(SixteenBySixteenSudoku),
            [
                "0123456789ABCDEF", "456789ABCDEF0123", "89ABCDEF01234567", "CDEF0123456789AB",
                "123456789ABCDEF0", "56789ABCDEF01234", "9ABCDEF012345678", "DEF0123456789ABC",
                "23456789ABCDEF01", "6789ABCDEF012345", "ABCDEF0123456789", "EF0123456789ABCD",
                "3456789ABCDEF012", "789ABCDEF0123456", "BCDEF0123456789A", "F0123456789ABCDE",
            ]
        },
        { nameof(FourByFourGrid), ["1234", "3412", "2341", "4123"] },
        { nameof(PentominoJigsaw), ["12345", "41523", "34251", "25134", "53412"] },
    };

    public static TheoryData<string, string[]> UnsolvedBoards => new()
    {
        // LC 37's solution with its last cell emptied.
        {
            nameof(ClassicSudoku),
            [
                "534678912", "672195348", "198342567", "859761423", "426853791",
                "713924856", "961537284", "287419635", "34528617.",
            ]
        },

        // LC 37's solution with its first row's first two digits swapped: the first two columns
        // repeat.
        {
            nameof(ClassicSudoku),
            [
                "354678912", "672195348", "198342567", "859761423", "426853791",
                "713924856", "961537284", "287419635", "345286179",
            ]
        },

        // The cyclic squares are Latin, but their regions repeat: the 6x6's first box holds 2
        // twice. In the 4x4's boxes the two cells that repeat come one after the other.
        { nameof(SixBySixSudoku), ["123456", "234561", "345612", "456123", "561234", "612345"] },
        { nameof(FourByFourGrid), ["1234", "2341", "3412", "4123"] },
        { nameof(PentominoJigsaw), ["12345", "23451", "34512", "45123", "51234"] },
    };

    [Theory]
    [MemberData(nameof(CellsToBoxNumbers))]
    public void RegionOf_Classic_NumbersBoxesRowByRow(int row, int col, int expected) =>
        Assert.Equal(expected, Sudoku<ClassicSudoku>.RegionOf(row, col));

    [Fact]
    public void RegionOf_Jigsaw_ReadsItsMap() =>
        Assert.Equal(2, Sudoku<PentominoJigsaw>.RegionOf(2, 0));

    [Fact]
    public void Size_Classic_IsNine() => Assert.Equal(9, Sudoku<ClassicSudoku>.Size);

    [Fact]
    public void Size_SixBySix_IsSix() => Assert.Equal(6, Sudoku<SixBySixSudoku>.Size);

    [Fact]
    public void Size_SixteenBySixteen_IsSixteen() =>
        Assert.Equal(16, Sudoku<SixteenBySixteenSudoku>.Size);

    [Fact]
    public void Size_FourByFourGrid_IsFour() => Assert.Equal(4, Sudoku<FourByFourGrid>.Size);

    [Fact]
    public void Size_Jigsaw_IsItsMapsOrder() => Assert.Equal(5, Sudoku<PentominoJigsaw>.Size);

    [Theory]
    [MemberData(nameof(PlacementsATopLeftFiveBlocks))]
    public void CanPlace_DigitItsRowColumnOrBoxHolds_ReturnsFalse(int row, int col, char digit) =>
        Assert.False(Sudoku<ClassicSudoku>.CanPlace(BoardWith((0, 0, '5')), row, col, digit));

    [Theory]
    [MemberData(nameof(PlacementsATopLeftFiveAllows))]
    public void CanPlace_DigitNoUnitHolds_ReturnsTrue(int row, int col, char digit) =>
        Assert.True(Sudoku<ClassicSudoku>.CanPlace(BoardWith((0, 0, '5')), row, col, digit));

    [Fact]
    public void CanPlace_FourByFourGrid_ReadsItsTwoByTwoBox()
    {
        var board = Board(["1...", "....", "....", "...."]);

        Assert.False(Sudoku<FourByFourGrid>.CanPlace(board, 1, 1, '1'));
        Assert.True(Sudoku<FourByFourGrid>.CanPlace(board, 2, 2, '1'));
    }

    // (1, 2) shares only the 2x3 box with the top-left cell, which a 3x2 box would not reach;
    // (2, 1) shares only the 3x2 box, which a 2x3 box does not reach.
    [Fact]
    public void CanPlace_SixBySix_ReadsItsTwoByThreeBox()
    {
        var board = Board(["1.....", "......", "......", "......", "......", "......"]);

        Assert.False(Sudoku<SixBySixSudoku>.CanPlace(board, 1, 2, '1'));
        Assert.True(Sudoku<SixBySixSudoku>.CanPlace(board, 2, 1, '1'));
    }

    // (1, 2) shares only region 0 with the top-left cell; (1, 1) sits in region 2 and shares
    // nothing.
    [Fact]
    public void CanPlace_Jigsaw_ReadsItsDrawnRegion()
    {
        var board = Board(["1....", ".....", ".....", ".....", "....."]);

        Assert.False(Sudoku<PentominoJigsaw>.CanPlace(board, 1, 2, '1'));
        Assert.True(Sudoku<PentominoJigsaw>.CanPlace(board, 1, 1, '1'));
    }

    [Fact]
    public void IsComplete_EveryCellFilled_ReturnsTrue() =>
        Assert.True(Sudoku<ClassicSudoku>.IsComplete(FilledBoard()));

    [Fact]
    public void IsComplete_OneEmptyCell_ReturnsFalse()
    {
        var board = FilledBoard();
        board[8][8] = ClassicSudoku.EmptyCell;

        Assert.False(Sudoku<ClassicSudoku>.IsComplete(board));
    }

    [Fact]
    public void IsComplete_EmptyBoard_ReturnsFalse() =>
        Assert.False(Sudoku<ClassicSudoku>.IsComplete(BoardWith()));

    [Theory]
    [MemberData(nameof(SolvedBoards))]
    public void IsSolved_EachSymbolOnceInEveryUnit_ReturnsTrue(string grid, string[] rows) =>
        Assert.True(IsSolvedOn(grid, Board(rows)));

    [Theory]
    [MemberData(nameof(UnsolvedBoards))]
    public void IsSolved_EmptyCellOrRepeat_ReturnsFalse(string grid, string[] rows) =>
        Assert.False(IsSolvedOn(grid, Board(rows)));

    private static bool IsSolvedOn(string grid, char[][] board) => grid switch
    {
        nameof(ClassicSudoku) => Sudoku<ClassicSudoku>.IsSolved(board),
        nameof(SixBySixSudoku) => Sudoku<SixBySixSudoku>.IsSolved(board),
        nameof(SixteenBySixteenSudoku) => Sudoku<SixteenBySixteenSudoku>.IsSolved(board),
        nameof(FourByFourGrid) => Sudoku<FourByFourGrid>.IsSolved(board),
        _ => Sudoku<PentominoJigsaw>.IsSolved(board),
    };

    private static char[][] Board(string[] rows) => [.. rows.Select(row => row.ToCharArray())];

    private static char[][] BoardWith(params (int Row, int Col, char Digit)[] cells)
    {
        var emptyRow = new string(ClassicSudoku.EmptyCell, ClassicSudoku.Size);
        var board = Board([.. Enumerable.Repeat(emptyRow, ClassicSudoku.Size)]);

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
}
