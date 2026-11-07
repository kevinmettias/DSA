using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch.PositionalGames;

// OpenLinesEvaluation's rule on an m,n,k board of up to 256 cells, Gomoku's among them: a line only
// one player has claimed cells of counts for that player by the cells it lacks, one short a threat
// (16) and two short (4), the mover's for and the opponent's against, and a line both have claimed
// in counts for nobody. OpenLinesEvaluation reads each winning set's mask in turn, which on
// Gomoku's 572 lines of five would be 572 tests of four words; here the lines are counted all at
// once by shifting the players' cells along the four line directions (InARowLines.CountLines), so
// an evaluation is a few dozen shifts whatever the board. The two read the same lines and weigh
// them the same, so on a board both hold they agree on every position. The overline rule plays no
// part: a line's count is an estimate either way.
/// <summary>
/// Estimates an m,n,k game's position from the lines each player alone holds cells of, one and
/// two cells short of a win, counted by shifting the board's masks.
/// </summary>
/// <typeparam name="TShape">The board's columns and rows, and how many in a row win.</typeparam>
/// <typeparam name="TOverline">Whether a run longer than k wins.</typeparam>
/// <param name="game">The game whose positions to estimate.</param>
public readonly struct ShiftedOpenLinesEvaluation<TShape, TOverline>(
    InARowGame<TShape, TOverline> game) : IStaticEvaluation<Bitboard256Position, int>
    where TShape : struct, IMnkShape
    where TOverline : struct, IOverlineRule
{
    /// <inheritdoc/>
    public int Evaluate(Bitboard256Position state)
    {
        // A count for every number of a player's cells a line can hold, none to all k.
        Span<int> mine = stackalloc int[TShape.InARow + 1];
        Span<int> theirs = stackalloc int[TShape.InARow + 1];
        game.Lines.CountLines(state.Mover, state.Opponent, mine);
        game.Lines.CountLines(state.Opponent, state.Mover, theirs);

        return Weighed(mine) - Weighed(theirs);
    }

    // The lines one cell short and two cells short of k, each at its weight; a line holding none
    // of the player's cells counts for nothing, as when k is 1 or 2.
    private static int Weighed(ReadOnlySpan<int> byHeld)
    {
        var oneShort = TShape.InARow - InARowLines<TShape, TOverline>.OneShort;
        var twoShort = TShape.InARow - InARowLines<TShape, TOverline>.TwoShort;
        var score = 0;

        if (oneShort > 0)
        {
            score += OpenLinesEvaluation.OneShortWeight * byHeld[oneShort];
        }

        if (twoShort > 0)
        {
            score += OpenLinesEvaluation.TwoShortWeight * byHeld[twoShort];
        }

        return score;
    }
}
