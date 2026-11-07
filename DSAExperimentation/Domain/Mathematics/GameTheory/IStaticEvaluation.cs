using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.GameTheory;

// Shannon's evaluation function (1950): what a search that stops before the end of the game takes
// a position to be worth, judged from the position alone. It is an estimate in the units of the
// game's own values and from the same side, the player to move's, so a search can put it where
// IAlternatingGame's value would go. It must lie strictly between the game's loss and its win, so
// that a result the search has forced always outranks a guess about one. Whatever a position
// holds that the rules do not score, such as lines a player is close to completing, is the
// estimate's to weigh; a search that reaches the end of the game never asks for one.
/// <summary>
/// An estimate of a position's value to the player to move, read from the position alone.
/// </summary>
/// <typeparam name="TState">A position, with the player to move implied.</typeparam>
/// <typeparam name="TScore">The game's score, in which the estimate is given.</typeparam>
public interface IStaticEvaluation<TState, TScore>
    where TScore : INumber<TScore>
{
    /// <summary>
    /// Estimates what a position the search will not look past is worth to the player to move.
    /// </summary>
    /// <param name="state">A position whose game is not over.</param>
    /// <returns>The estimate, strictly between the game's loss and its win.</returns>
    TScore Evaluate(TState state);
}
