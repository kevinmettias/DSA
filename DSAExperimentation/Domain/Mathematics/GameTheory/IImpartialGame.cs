namespace DSAExperimentation.Domain.Mathematics.GameTheory;

// A game in which both players have the same moves from every position, as in Nim, so the position
// alone decides the game. By the Sprague-Grundy theorem each position has a Grundy number, the minimum
// excludant of the numbers of the positions it moves to; under normal play, where the player left
// without a move loses, the player to move wins exactly when that number is not zero, and a sum of
// independent games has the exclusive or of its parts' numbers. Moves are numbered 0 to MoveCount - 1,
// and the rules are an instance struct carrying the problem's input, as in IAlternatingGame.
/// <summary>A game in which both players have the same moves from every position.</summary>
/// <typeparam name="TState">A position.</typeparam>
public interface IImpartialGame<TState>
{
    /// <summary>Counts the moves from a position.</summary>
    /// <param name="state">The position.</param>
    /// <returns>How many moves there are; zero when the game is over.</returns>
    int MoveCount(TState state);

    /// <summary>Finds the position a move leaves.</summary>
    /// <param name="state">The position before the move.</param>
    /// <param name="move">The move's number, from 0 below <see cref="MoveCount"/>.</param>
    /// <returns>The position after the move.</returns>
    TState Next(TState state, int move);
}
