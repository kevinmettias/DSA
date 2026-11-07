using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch;

// Solves a two-player alternating game by negamax: a position is worth the best, over its moves, of the
// move's gain less what the opponent can force from the position it leaves, and a finished position is
// worth what its rules say. One recursion serves both players because each value is the mover's, so the
// opponent's is negated rather than minimized. Positions recur along different move orders (in a stone
// game, taking one stone then two reaches the position taking three does), so the recursion runs through
// Memoizer and each position is valued once. A cycle among positions overflows the stack, as Memoizer's
// law says; a game that can repeat a position needs retrograde analysis instead. A game too large
// to value every position of is searched to a fixed depth, with an estimate at the limit:
// ValueToDepth runs DepthLimitedSearch, the recursion AlphaBeta runs, with no cutoff, so it is the
// baseline alpha-beta's pruning is measured against.
/// <summary>Values a two-player alternating game by negamax over memoized positions.</summary>
public static class Negamax
{
    /// <summary>Finds the value of a position to the player to move: the most they can force.</summary>
    /// <param name="game">The game's rules, carrying the problem's input.</param>
    /// <param name="start">The position to value.</param>
    /// <returns>The value of <paramref name="start"/> to the player to move.</returns>
    public static TScore Value<TGame, TState, TScore>(TGame game, TState start)
        where TGame : struct, IAlternatingGame<TState, TScore>
        where TState : notnull
        where TScore : INumber<TScore>
        => Memoizer.Memoize(start, new BestReply<TGame, TState, TScore>(game));

    /// <summary>
    /// Finds a position's value to the player to move by plain negamax to a fixed depth, every move
    /// of every position within it searched.
    /// </summary>
    /// <param name="game">The game's rules, carrying the problem's input.</param>
    /// <param name="evaluation">The estimate of a position the search will not look past.</param>
    /// <param name="start">The position to value.</param>
    /// <param name="depth">How many plies to look ahead; 0 evaluates the position itself.</param>
    /// <returns>
    /// The value of <paramref name="start"/> to the player to move at that depth.
    /// </returns>
    public static TScore ValueToDepth<TGame, TEvaluation, TState, TScore>(
        TGame game, TEvaluation evaluation, TState start, int depth)
        where TGame : struct, IAlternatingGame<TState, TScore>
        where TEvaluation : struct, IStaticEvaluation<TState, TScore>
        where TScore : INumber<TScore>, IMinMaxValue<TScore>
    {
        var search = new DepthLimitedSearch<
            TGame, TEvaluation, MoveNumberOrder<TState>, NoCutoff<TScore>,
            NoTranspositions<TState, TScore>, TState, TScore>(game, evaluation, default, default);

        return search.ValueOf(start, depth);
    }

    // One position's value from its moves' values, each found through the memoized recursion.
    private sealed class BestReply<TGame, TState, TScore>(TGame game) : IRecurrence<TState, TScore>
        where TGame : struct, IAlternatingGame<TState, TScore>
        where TScore : INumber<TScore>
    {
        public TScore Replay(TState state, IRecurrence<TState, TScore> rest)
        {
            var moveCount = game.MoveCount(state);

            if (moveCount == 0)
            {
                return game.ValueWhenOver(state);
            }

            var best = ValueOf(state, 0, rest);

            for (var move = 1; move < moveCount; move++)
            {
                var value = ValueOf(state, move, rest);
                best = TScore.Max(best, value);
            }

            return best;
        }

        // A move's worth to its mover: what it gains, less what the opponent can force after it.
        private TScore ValueOf(TState state, int move, IRecurrence<TState, TScore> rest)
        {
            var after = game.Next(state, move);

            return game.Gain(state, move) - rest.Replay(after, rest);
        }
    }
}
