namespace DSAExperimentation.Domain.Mathematics.GameTheory;

// A finite two-player game in which the players alternate and every outcome is the mover's, seen
// as a graph: its positions are the vertices and its moves the edges. Play may return to a
// position, which IAlternatingGame's memoized solvers cannot follow, so it can go on for ever, and
// a position neither player can force to an end is drawn. A solver that values such a game works
// backwards from the positions its rules decide, so it must visit every position and every move,
// which a recursion forward from one position cannot do once play can cycle. The game therefore
// numbers its positions 0 to PositionCount - 1, and a solver keeps what it learns in arrays
// indexed by that number rather than in a dictionary. A game whose positions are those numbers
// maps each to itself.
//
// The rules are an instance struct carrying the problem's input, and moves are numbered 0 to
// MoveCount - 1, as in IAlternatingGame. A move only wins or loses, so there is no gain, and a
// position with no move left is over, worth what its rules say: a loss to the player who cannot
// move under normal play, a draw when the rules call it one.
/// <summary>
/// A finite two-player game whose positions are numbered and whose play may return to a position.
/// </summary>
/// <typeparam name="TPosition">
/// A position, with the player to move implied: every outcome is the mover's.
/// </typeparam>
public interface IGameGraph<TPosition>
{
    /// <summary>Gets how many positions the game has.</summary>
    int PositionCount { get; }

    /// <summary>Finds the position a number stands for.</summary>
    /// <param name="index">The position's number, from 0 below <see cref="PositionCount"/>.</param>
    /// <returns>The position numbered <paramref name="index"/>.</returns>
    TPosition PositionAt(int index);

    /// <summary>Finds a position's number.</summary>
    /// <param name="position">A position of the game.</param>
    /// <returns>Its number, from 0 below <see cref="PositionCount"/>.</returns>
    int IndexOf(TPosition position);

    /// <summary>Counts the moves the player to move may make.</summary>
    /// <param name="position">The position.</param>
    /// <returns>How many moves there are; zero when the game is over.</returns>
    int MoveCount(TPosition position);

    /// <summary>Finds the position a move leaves, with the other player to move.</summary>
    /// <param name="position">The position before the move.</param>
    /// <param name="move">The move's number, from 0 below <see cref="MoveCount"/>.</param>
    /// <returns>The position after the move.</returns>
    TPosition Next(TPosition position, int move);

    /// <summary>
    /// Finds the outcome of a position with no move left, for the player who cannot move.
    /// </summary>
    /// <param name="position">The finished position.</param>
    /// <returns>Its outcome to that player, such as a loss under normal play.</returns>
    GameOutcome OutcomeWhenOver(TPosition position);
}
