using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Domain.Games.PositionalGames;

// The layer a positional game's bit-mask forms share: the game contract a solver searches, with a
// position set up by replaying choices from the empty board and a move read back as its choice.
// BitboardGame meets it on boards of at most 64 cells and any winning sets; InARowGame, the
// strategy for boards beyond, meets it on m,n,k boards of up to 256 cells, its lines read by
// shifting. What the layer guarantees, whichever form: two players alternate, the first to
// complete a winning set wins and a board with no choice left is drawn; moves are numbered by
// their choices, lowest first; a win is worth BitboardGame.WinValue less the claimed cells and a
// loss its negation, so a quicker win is worth more; a move gains nothing; and Replay refuses a
// choice that is not open or that follows the end of the game. One suite holds both forms to it
// (BitboardGameContractTests).
/// <summary>
/// A positional game on a bit-mask board as a solver plays it: the game contract, with positions
/// set up from choices and moves read back as choices.
/// </summary>
/// <typeparam name="TState">A position, with the player to move implied.</typeparam>
public interface IBitboardGame<TState> : IAlternatingGame<TState, int>
{
    /// <summary>Names the choice a move makes, as the placement rule names it.</summary>
    /// <param name="position">A position of this game.</param>
    /// <param name="move">The move's number, from 0 below the position's move count.</param>
    /// <returns>The choice: a cell under free placement, a column under gravity.</returns>
    int ChoiceOf(TState position, int move);

    /// <summary>Plays choices from the empty board and returns the position they reach.</summary>
    /// <param name="choices">The choices in order, named as the placement rule names them.</param>
    /// <returns>The position after the last choice.</returns>
    /// <exception cref="ArgumentException">
    /// A choice is not open, or follows the end of the game.
    /// </exception>
    TState Replay(ReadOnlySpan<int> choices);
}
