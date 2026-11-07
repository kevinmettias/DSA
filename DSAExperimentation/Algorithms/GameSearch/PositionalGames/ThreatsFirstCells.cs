using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch.PositionalGames;

// Tries an m,n,k game's moves threats first, the order a Gomoku player reads a board in: a cell
// that wins at once; a cell where the opponent would win next move, which must be blocked; a cell
// that makes a four, one cell short of a win; a cell where the opponent would make one; a cell that
// makes a three; then the cells next to a stone, the board's centre first when it is empty; and
// last every other cell. A cell keeps the first rank it earns, and within a rank the cells go
// lowest first. Each rank is a mask the board's lines give at once (InARowLines), so the order
// costs a few dozen shifts, and the ranks sit in a span on the stack. A move is the game's number
// for its cell, the count of open cells below it. The contract wants every move once, so the far
// cells come last rather than not at all: against alpha-beta that tries the threats first they
// are refuted at once, but a search still visits each one where it cannot cut off.
/// <summary>
/// Orders an m,n,k game's moves by threat: wins, blocks, fours and threes first, then the cells
/// next to a stone, then the rest.
/// </summary>
/// <typeparam name="TShape">The board's columns and rows, and how many in a row win.</typeparam>
/// <typeparam name="TOverline">Whether a run longer than k wins.</typeparam>
/// <param name="game">The game whose moves to order.</param>
public readonly struct ThreatsFirstCells<TShape, TOverline>(InARowGame<TShape, TOverline> game)
    : IMoveOrdering<Bitboard256Position>
    where TShape : struct, IMnkShape
    where TOverline : struct, IOverlineRule
{
    private const int Wins = InARowLines<TShape, TOverline>.OneShort;
    private const int MakesAFour = InARowLines<TShape, TOverline>.TwoShort;
    private const int MakesAThree = InARowLines<TShape, TOverline>.ThreeShort;

    // The board's middle cell, where an empty board's search starts.
    private static readonly int Centre = CentreBit();

    /// <inheritdoc/>
    public void Order(Bitboard256Position state, Span<int> moves)
    {
        var lines = game.Lines;
        var (mover, opponent) = (state.Mover, state.Opponent);
        var open = game.OpenCells(state);
        ReadOnlySpan<Bitboard256> ranks =
        [
            lines.CellsLacking(mover, opponent, Wins),
            lines.CellsLacking(opponent, mover, Wins),
            lines.CellsLacking(mover, opponent, MakesAFour),
            lines.CellsLacking(opponent, mover, MakesAFour),
            lines.CellsLacking(mover, opponent, MakesAThree),
            Near(lines, state.Claimed),
            open,
        ];

        WriteByRank(ranks, open, moves);
    }

    // The cells next to a stone, or the centre when the board is empty.
    private static Bitboard256 Near(InARowLines<TShape, TOverline> lines, Bitboard256 claimed)
    {
        if (claimed.IsEmpty)
        {
            return Bitboard256.Cell(Centre);
        }

        return lines.Around(claimed);
    }

    // Writes the move of each open cell once, rank by rank, each rank's cells lowest first.
    private static void WriteByRank(
        ReadOnlySpan<Bitboard256> ranks, Bitboard256 open, Span<int> moves)
    {
        var left = open;
        var written = 0;

        foreach (var rank in ranks)
        {
            var cells = rank & left;
            left &= ~cells;

            for (; !cells.IsEmpty; cells = cells.WithoutLowestCell())
            {
                moves[written++] = open.CountBelow(cells.LowestCell);
            }
        }
    }

    private static int CentreBit()
    {
        // Half way down and half way across, the left and upper of two middles.
        var cell = MnkGame<TShape>.CellAt(TShape.Rows >> 1, TShape.Columns >> 1);

        return InARowLines<TShape, TOverline>.BitOf(cell);
    }
}
