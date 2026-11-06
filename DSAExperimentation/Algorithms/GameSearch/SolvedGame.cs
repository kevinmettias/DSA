using System.Collections.Immutable;
using System.Runtime.InteropServices;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch;

// Every position of a game valued: its outcome for the player to move, and how many moves the game
// lasts from it when both play best, the winner ending it as soon as they can and the loser holding
// out as long as they can, as an endgame table records it. That length is what lets a game cut off
// after so many moves be answered without the count of moves made in its positions: the player to
// move wins within the cut-off exactly when the position is a win that lasts no longer. Both are
// kept by the game's own numbering of its positions (IGameGraph), so a lookup is an index.
// RetrogradeAnalysis builds it, and once built it does not change, so a copy shares it.
/// <summary>
/// Every position of a game valued as a win, a loss or a draw, with how long best play lasts from
/// it.
/// </summary>
public readonly struct SolvedGame
{
    private readonly ImmutableArray<GameOutcome> _outcomes;
    private readonly ImmutableArray<int> _movesToEnd;

    internal SolvedGame(GameOutcome[] outcomes, int[] movesToEnd)
    {
        _outcomes = ImmutableCollectionsMarshal.AsImmutableArray(outcomes);
        _movesToEnd = ImmutableCollectionsMarshal.AsImmutableArray(movesToEnd);
    }

    /// <summary>Finds a position's outcome for the player to move.</summary>
    /// <param name="index">The position's number in its game.</param>
    /// <returns>
    /// A win when that player can force one, a loss when the opponent can, a draw when neither can.
    /// </returns>
    public GameOutcome OutcomeOf(int index) => _outcomes[index];

    /// <summary>
    /// Finds how many moves the game lasts from a position when both players play best.
    /// </summary>
    /// <param name="index">The position's number in its game.</param>
    /// <returns>
    /// For a won or lost position, the moves until the game ends: as few as the winner can force
    /// and as many as the loser can hold out for. Zero for a position already over, and for a
    /// drawn one.
    /// </returns>
    public int MovesToEnd(int index) => _movesToEnd[index];
}
