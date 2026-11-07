using DSAExperimentation.Algorithms.ExactCover;
using DSAExperimentation.DataStructures.Alphabets;
using DSAExperimentation.Domain.Games.Sudoku;
using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;
using DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku;

// The reduction's numbers, then the reduction proved by solving: Algorithm X's cover of a puzzle's
// matrix, written back onto the board, must be a solved board (Sudoku.IsSolved) keeping every
// given, on a box grid of each shape and on the jigsaw. The 4x4 grid has 288 solutions, half its
// 576 Latin squares, which only the region constraints tell apart. Puzzles other than LC 37's are
// solved boards with every other cell emptied, as a checkerboard.
public sealed partial class SudokuCoverTests
{
    private const int FourByFourSolutions = 288;

    // A 6x6 board whose one given, 7, is not among its six symbols.
    private static readonly string[] SevenOnASixBySix =
        ["7.....", "......", "......", "......", "......", "......"];

    // No given repeats, but the first row's last cell can take only 9, which its column holds.
    private static readonly string[] Unsolvable =
    [
        "12345678.", "........9", ".........", ".........", ".........", ".........", ".........",
        ".........", ".........",
    ];

    [Fact]
    public void PrimaryColumnCount_Classic_IsFourFamiliesOfEightyOne() =>
        Assert.Equal(4 * 81, default(SudokuCover<ClassicSudoku>).PrimaryColumnCount);

    [Fact]
    public void SecondaryColumnCount_AnyGrid_IsZero() =>
        Assert.Equal(0, default(SudokuCover<PentominoJigsaw>).SecondaryColumnCount);

    [Fact]
    public void RowCount_SixBySix_IsEveryPlacement() =>
        Assert.Equal(6 * 6 * 6, default(SudokuCover<SixBySixSudoku>).RowCount);

    [Fact]
    public void ColumnCountOf_AnyPlacement_IsFour() =>
        Assert.Equal(4, default(SudokuCover<ClassicSudoku>).ColumnCountOf(371));

    // A 3 (symbol 2) in row 4, column 5, which lies in box 4.
    [Fact]
    public void WriteColumns_ClassicPlacement_MeetsItsCellRowColumnAndBox()
    {
        var columns = new int[4];

        default(SudokuCover<ClassicSudoku>).WriteColumns(371, columns);

        Assert.Equal([41, 81 + 38, 162 + 47, 243 + 38], columns);
    }

    // A 2 (symbol 1) in row 2, column 0, which the jigsaw's map puts in region 2.
    [Fact]
    public void WriteColumns_JigsawPlacement_ReadsTheRegionMap()
    {
        var columns = new int[4];
        var placement = SudokuCover<PentominoJigsaw>.PlacementRow(2, 0, 1);

        default(SudokuCover<PentominoJigsaw>).WriteColumns(placement, columns);

        Assert.Equal([10, 25 + 11, 50 + 1, 75 + 11], columns);
    }

    [Fact]
    public void PlacementRow_Classic_NumbersCellThenSymbol() =>
        Assert.Equal(371, SudokuCover<ClassicSudoku>.PlacementRow(4, 5, 2));

    [Fact]
    public void PlacementOf_PlacementRow_RoundTrips() =>
        Assert.Equal((4, 5, 2), SudokuCover<ClassicSudoku>.PlacementOf(371));

    [Fact]
    public void GivenRows_Board_ListsEachGivenRowByRow()
    {
        var board = Board(["5........", "........3", .. Enumerable.Repeat(".........", 7)]);

        Assert.Equal([4, 155], SudokuCover<ClassicSudoku>.GivenRows<SudokuDigits>(board));
    }

    [Fact]
    public void GivenRows_SymbolPastTheGridsLast_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SudokuCover<SixBySixSudoku>.GivenRows<SudokuDigits>(Board(SevenOnASixBySix)));

    [Fact]
    public void Fill_Placements_WritesEachSymbol()
    {
        var board = Board(["....", "....", "....", "...."]);

        SudokuCover<FourByFourGrid>.Fill<SudokuDigits>(board, [0, 63]);

        Assert.Equal(["1...", "....", "....", "...4"], board.Select(row => new string(row)));
    }

    [Fact]
    public void SolvedBoard_Classic_SolvesLeetCodesExample() =>
        AssertSolvesKeepingGivens<ClassicSudoku, SudokuDigits>(SudokuBoards.LeetCodeExample);

    [Fact]
    public void SolvedBoard_SixBySix_SolvesItsTwoByThreeBoxes() =>
        AssertSolvesKeepingGivens<SixBySixSudoku, SudokuDigits>(Checkerboard(SudokuBoards.SolvedSixBySix));

    [Fact]
    public void SolvedBoard_SixteenBySixteen_SolvesItsFourByFourBoxes() =>
        AssertSolvesKeepingGivens<SixteenBySixteenSudoku, ZeroToFSymbols>(
            Checkerboard(SudokuBoards.SolvedSixteenBySixteen));

    [Fact]
    public void SolvedBoard_Jigsaw_SolvesItsDrawnRegions() =>
        AssertSolvesKeepingGivens<PentominoJigsaw, SudokuDigits>(Checkerboard(SudokuBoards.SolvedJigsaw));

    [Fact]
    public void SolvedBoard_ClassicWithNoGivens_SolvesAnEmptyBoard() =>
        AssertSolvesKeepingGivens<ClassicSudoku, SudokuDigits>(
            [.. Enumerable.Repeat(".........", 9)]);

    [Fact]
    public void SolvedBoard_Unsolvable_FindsNoCover()
    {
        var givens = SudokuCover<ClassicSudoku>.GivenRows<SudokuDigits>(Board(Unsolvable));

        Assert.False(AlgorithmX.TryFindCover(default(SudokuCover<ClassicSudoku>), givens, out _));
    }

    [Fact]
    public void SolvedBoard_GivensRepeatingInARow_FindsNoCover()
    {
        var board = Board(["55.......", .. Enumerable.Repeat(".........", 8)]);
        var givens = SudokuCover<ClassicSudoku>.GivenRows<SudokuDigits>(board);

        Assert.False(AlgorithmX.TryFindCover(default(SudokuCover<ClassicSudoku>), givens, out _));
    }

    [Fact]
    public void CountCovers_EmptyFourByFour_IsTheGridsSolutionsNotItsLatinSquares() =>
        Assert.Equal(
            FourByFourSolutions, AlgorithmX.CountCovers(default(SudokuCover<FourByFourGrid>), []));

    // The puzzle solved by exact cover is a solved board that keeps every given.
    private static void AssertSolvesKeepingGivens<TGrid, TSymbols>(string[] puzzle)
        where TGrid : struct, ISudokuGrid
        where TSymbols : struct, IAlphabet
    {
        var board = SolvedByExactCover<TGrid, TSymbols>(puzzle);

        Assert.True(Sudoku<TGrid>.IsSolved(board));
        Assert.All(
            GivenCells(puzzle), cell => Assert.Equal(cell.Symbol, board[cell.Row][cell.Col]));
    }

    // The puzzle's board with Algorithm X's cover of its matrix written onto it, once the cover
    // is found and keeps the exact cover rule.
    private static char[][] SolvedByExactCover<TGrid, TSymbols>(string[] puzzle)
        where TGrid : struct, ISudokuGrid
        where TSymbols : struct, IAlphabet
    {
        var board = Board(puzzle);
        var givens = SudokuCover<TGrid>.GivenRows<TSymbols>(board);

        Assert.True(AlgorithmX.TryFindCover(default(SudokuCover<TGrid>), givens, out var cover));
        Assert.True(ExactCover<SudokuCover<TGrid>>.IsCover(default, cover));
        SudokuCover<TGrid>.Fill<TSymbols>(board, cover);

        return board;
    }

    private static IEnumerable<(int Row, int Col, char Symbol)> GivenCells(string[] puzzle) =>
        puzzle.SelectMany((cells, row) => cells.Select((symbol, col) => (row, col, symbol)))
            .Where(cell => cell.symbol != ClassicSudoku.EmptyCell);

    // A solved board with the cells whose row and column add to an odd number emptied.
    private static string[] Checkerboard(string[] solved) =>
        [.. solved.Select((cells, row) => new string([.. cells.Select((symbol, col) =>
            KeptOnACheckerboard(row, col, symbol))]))];

    private static char KeptOnACheckerboard(int row, int col, char symbol)
    {
        var isKept = (row + col) % 2 == 0;

        return isKept ? symbol : ClassicSudoku.EmptyCell;
    }

    private static char[][] Board(string[] rows) => [.. rows.Select(row => row.ToCharArray())];
}
