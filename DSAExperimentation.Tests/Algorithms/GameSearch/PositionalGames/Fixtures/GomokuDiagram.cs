using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames.Fixtures;

// A Gomoku position drawn as its fifteen rows, top first, test-only: X for the player to move, O
// for the other, and a dot for an open cell.
internal static class GomokuDiagram
{
    private const char Mover = 'X';
    private const char Opponent = 'O';

    // The cells of each kind, numbered row by row as MnkGame numbers them.
    public static (int[] Mover, int[] Opponent) Cells(string[] rows) =>
        (CellsOf(rows, Mover), CellsOf(rows, Opponent));

    // The position, with X to move and the game going on.
    public static Bitboard256Position Position(string[] rows)
    {
        var (mover, opponent) = Cells(rows);

        return new(Mask(mover), Mask(opponent), IsLost: false);
    }

    public static int Cell(int row, int column) => MnkGame<GomokuBoard>.CellAt(row, column);

    private static Bitboard256 Mask(int[] cells) =>
        InARowLines<GomokuBoard, KOrMore>.MaskOf(cells);

    private static int[] CellsOf(string[] rows, char kind) =>
    [
        .. Enumerable.Range(0, GomokuBoard.Size).SelectMany(row => Enumerable
            .Range(0, GomokuBoard.Size)
            .Where(column => rows[row][column] == kind)
            .Select(column => Cell(row, column))),
    ];
}
