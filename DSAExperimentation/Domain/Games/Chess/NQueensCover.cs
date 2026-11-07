using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;

namespace DSAExperimentation.Domain.Games.Chess;

// The n-queens puzzle (Bezzel's eight queens, 1848, on any n x n board) as an exact cover problem,
// the example Knuth's dancing-links paper gives secondary columns for. A row of the matrix puts a
// queen on one square, and meets the four lines a queen rides through it: its row, its column, its
// diagonal and its anti-diagonal (ChessPiece's queen rides all four ways). Each of the n rows and
// n columns must hold exactly one queen, since n queens no two of which share one fill them all,
// so those 2n lines are primary. A diagonal may hold one queen or none, so its 2n - 1 lines each
// way are secondary. A cover is then a placement of n queens no two attacking. Squares are numbered
// row by row, (row x n) + column. The board's side is the puzzle's input, so the problem is an
// instance struct carrying it.
/// <summary>The n-queens puzzle as an exact cover: squares as rows, lines as columns.</summary>
public readonly struct NQueensCover : IExactCoverProblem
{
    // The lines through a square, in the order a square lists them.
    private const int RowLine = 0;
    private const int ColumnLine = 1;
    private const int DiagonalLine = 2;
    private const int AntiDiagonalLine = 3;
    private const int LinesPerSquare = AntiDiagonalLine + 1;

    private readonly int _boardSize;

    // How many diagonals run each way across the board.
    private int DiagonalsEachWay => _boardSize + _boardSize - 1;

    /// <summary>Gets the board's side, n.</summary>
    public int BoardSize => _boardSize;

    /// <summary>Gets the board's rows and columns, n of each, each holding one queen.</summary>
    public int PrimaryColumnCount => _boardSize + _boardSize;

    /// <summary>Gets the diagonals, 2n - 1 each way, each holding a queen at most.</summary>
    public int SecondaryColumnCount => DiagonalsEachWay + DiagonalsEachWay;

    /// <summary>Gets the squares, n x n.</summary>
    public int RowCount => _boardSize * _boardSize;

    /// <summary>Initializes the puzzle on a board of a given side.</summary>
    /// <param name="boardSize">The board's side, n, which is also how many queens to place.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="boardSize"/> is negative.
    /// </exception>
    public NQueensCover(int boardSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(boardSize);
        _boardSize = boardSize;
    }

    /// <summary>Counts the lines through a square: four.</summary>
    /// <param name="row">The square.</param>
    /// <returns>Four.</returns>
    public int ColumnCountOf(int row) => LinesPerSquare;

    /// <summary>Writes the lines through a square: row, column, diagonal, anti-diagonal.</summary>
    /// <param name="row">The square, numbered row by row.</param>
    /// <param name="columns">Four entries, for the four lines.</param>
    public void WriteColumns(int row, Span<int> columns)
    {
        var (boardRow, boardCol) = SquareOf(row);
        var firstDiagonal = PrimaryColumnCount;
        var firstAntiDiagonal = firstDiagonal + DiagonalsEachWay;

        columns[RowLine] = boardRow;
        columns[ColumnLine] = _boardSize + boardCol;
        columns[DiagonalLine] = firstDiagonal + (boardRow - boardCol + _boardSize - 1);
        columns[AntiDiagonalLine] = firstAntiDiagonal + boardRow + boardCol;
    }

    /// <summary>Finds the row and column of a square.</summary>
    /// <param name="square">The square, numbered row by row from 0.</param>
    /// <returns>The square's row and column.</returns>
    public (int Row, int Col) SquareOf(int square) => (square / _boardSize, square % _boardSize);
}
