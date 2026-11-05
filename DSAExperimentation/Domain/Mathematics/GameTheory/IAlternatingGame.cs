using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.GameTheory;

// A two-player game of perfect information in which the players alternate, valued from the side of the
// player to move: the most they can force. A move scores Gain for the player who makes it, and the
// value of a state is the best, over its moves, of the gain less the value the opponent can force from
// the state the move leaves; a state with no move left is worth ValueWhenOver to the player who cannot
// move. Every two-player zero-sum game of that kind fits. A win-or-lose game gains nothing and ends at
// -1 for the player left without a move under normal play (+1 under misere play), so the player to
// move wins exactly when the value is positive; a scoring game, such as the stone games, gains what
// each move takes, and the value is the score difference the player to move can force.
//
// The rules are an instance struct rather than a static witness, so they carry the problem's own input
// (a row of stones, the numbers still unused), and a solver generic over them compiles each game's
// rules into its own code. Moves are numbered 0 to MoveCount - 1, so no move list is allocated.
/// <summary>A two-player game in which the players alternate, valued as the difference the player to move can force.</summary>
/// <typeparam name="TState">A position, with the player to move implied: every value is the mover's.</typeparam>
/// <typeparam name="TScore">The score a move gains and a position is worth.</typeparam>
public interface IAlternatingGame<TState, TScore>
    where TScore : INumber<TScore>
{
    /// <summary>Counts the moves the player to move may make.</summary>
    /// <param name="state">The position.</param>
    /// <returns>How many moves there are; zero when the game is over.</returns>
    int MoveCount(TState state);

    /// <summary>Finds the position a move leaves, with the other player to move.</summary>
    /// <param name="state">The position before the move.</param>
    /// <param name="move">The move's number, from 0 below <see cref="MoveCount"/>.</param>
    /// <returns>The position after the move.</returns>
    TState Next(TState state, int move);

    /// <summary>Finds what a move scores for the player who makes it.</summary>
    /// <param name="state">The position before the move.</param>
    /// <param name="move">The move's number, from 0 below <see cref="MoveCount"/>.</param>
    /// <returns>The score the move gains; zero in a game won or lost only at its end.</returns>
    TScore Gain(TState state, int move);

    /// <summary>Finds what a position with no move left is worth to the player who cannot move.</summary>
    /// <param name="state">The finished position.</param>
    /// <returns>Its value to that player, such as -1 for a loss under normal play.</returns>
    TScore ValueWhenOver(TState state);
}
