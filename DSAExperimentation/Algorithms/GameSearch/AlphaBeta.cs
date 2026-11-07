using System.Numerics;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch;

// Values a position of a game too large to value every position of: negamax to a fixed depth,
// with a static evaluation in place of the value at the depth limit, and alpha-beta's cutoff. A
// move that refutes the opponent's choice ends the search of the opponent's remaining moves, so
// with the best move tried first the search looks at about the square root of the positions plain
// negamax does (Knuth and Moore, 1975), and finds the same value. The recursion is
// DepthLimitedSearch's, the one Negamax.ValueToDepth runs without the cutoff, so the cutoff is all
// that sets the two apart. The order moves are tried in is a witness of its own: it changes how
// much is searched, never the value. The game, the evaluation and the ordering are structs, so a
// search compiles them into its own code. Iterative deepening searches to each depth in turn and
// remembers every position in a transposition table, so each pass tries first the moves the last
// found best, and positions reached by two move orders are searched once a depth; on Connect Four
// that searches fewer positions than one pass to the last depth without the table does.
/// <summary>
/// Values a two-player alternating game to a fixed depth by negamax with alpha-beta pruning.
/// </summary>
public static class AlphaBeta
{
    private const string GameOver = "The game is over: there is no move to choose.";

    /// <summary>
    /// Finds a position's value to the player to move, searched to a fixed depth.
    /// </summary>
    /// <typeparam name="TGame">The game's rules.</typeparam>
    /// <typeparam name="TEvaluation">The estimate of a position at the depth limit.</typeparam>
    /// <typeparam name="TOrdering">The order in which to try each position's moves.</typeparam>
    /// <typeparam name="TState">A position.</typeparam>
    /// <typeparam name="TScore">The game's score.</typeparam>
    /// <param name="game">The game's rules, carrying the problem's input.</param>
    /// <param name="evaluation">The estimate of a position the search will not look past.</param>
    /// <param name="ordering">The order in which to try each position's moves.</param>
    /// <param name="start">The position to value.</param>
    /// <param name="depth">How many plies to look ahead; 0 evaluates the position itself.</param>
    /// <returns>
    /// The value of <paramref name="start"/> to the player to move, as plain negamax to the same
    /// depth finds it.
    /// </returns>
    public static TScore Value<TGame, TEvaluation, TOrdering, TState, TScore>(
        TGame game, TEvaluation evaluation, TOrdering ordering, TState start, int depth)
        where TGame : struct, IAlternatingGame<TState, TScore>
        where TEvaluation : struct, IStaticEvaluation<TState, TScore>
        where TOrdering : struct, IMoveOrdering<TState>
        where TScore : INumber<TScore>, IMinMaxValue<TScore>
    {
        var search = new DepthLimitedSearch<
            TGame, TEvaluation, TOrdering, AlphaBetaCutoff<TScore>,
            NoTranspositions<TState, TScore>, TState, TScore>(game, evaluation, ordering, default);

        return search.ValueOf(start, depth);
    }

    /// <summary>
    /// Finds the best move from a position and its value, searched to a fixed depth.
    /// </summary>
    /// <typeparam name="TGame">The game's rules.</typeparam>
    /// <typeparam name="TEvaluation">The estimate of a position at the depth limit.</typeparam>
    /// <typeparam name="TOrdering">The order in which to try each position's moves.</typeparam>
    /// <typeparam name="TState">A position.</typeparam>
    /// <typeparam name="TScore">The game's score.</typeparam>
    /// <param name="game">The game's rules, carrying the problem's input.</param>
    /// <param name="evaluation">The estimate of a position the search will not look past.</param>
    /// <param name="ordering">The order in which to try each position's moves.</param>
    /// <param name="start">The position to move from, with at least one move.</param>
    /// <param name="depth">How many plies to look ahead, at least 1.</param>
    /// <returns>
    /// The best move's number and its value; of moves worth the same, the one tried first.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="depth"/> is below 1.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="start"/> has no move.</exception>
    public static (int Move, TScore Value) BestMove<TGame, TEvaluation, TOrdering, TState, TScore>(
        TGame game, TEvaluation evaluation, TOrdering ordering, TState start, int depth)
        where TGame : struct, IAlternatingGame<TState, TScore>
        where TEvaluation : struct, IStaticEvaluation<TState, TScore>
        where TOrdering : struct, IMoveOrdering<TState>
        where TScore : INumber<TScore>, IMinMaxValue<TScore>
    {
        CheckMoveSearch<TGame, TState, TScore>(game, start, depth);
        var search = new DepthLimitedSearch<
            TGame, TEvaluation, TOrdering, AlphaBetaCutoff<TScore>,
            NoTranspositions<TState, TScore>, TState, TScore>(game, evaluation, ordering, default);

        return search.BestMove(start, depth);
    }

    /// <summary>
    /// Finds the best move from a position and its value by iterative deepening: a search to each
    /// depth in turn, from 1, remembering every position searched in a transposition table.
    /// </summary>
    /// <typeparam name="TGame">The game's rules.</typeparam>
    /// <typeparam name="TEvaluation">The estimate of a position at the depth limit.</typeparam>
    /// <typeparam name="TOrdering">The order in which to try each position's moves.</typeparam>
    /// <typeparam name="TState">
    /// A position, with equality that says when two are the same.
    /// </typeparam>
    /// <typeparam name="TScore">The game's score.</typeparam>
    /// <param name="game">The game's rules, carrying the problem's input.</param>
    /// <param name="evaluation">The estimate of a position the search will not look past.</param>
    /// <param name="ordering">
    /// The order in which to try each position's moves, after the best move an earlier pass found.
    /// </param>
    /// <param name="start">The position to move from, with at least one move.</param>
    /// <param name="depth">How many plies the last pass looks ahead, at least 1.</param>
    /// <returns>
    /// The last pass's best move and its value, the value <see cref="BestMove"/> finds at that
    /// depth.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="depth"/> is below 1.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="start"/> has no move.</exception>
    public static (int Move, TScore Value) BestMoveByDeepening<
        TGame, TEvaluation, TOrdering, TState, TScore>(
        TGame game, TEvaluation evaluation, TOrdering ordering, TState start, int depth)
        where TGame : struct, IAlternatingGame<TState, TScore>
        where TEvaluation : struct, IStaticEvaluation<TState, TScore>
        where TOrdering : struct, IMoveOrdering<TState>
        where TState : notnull
        where TScore : INumber<TScore>, IMinMaxValue<TScore>
    {
        CheckMoveSearch<TGame, TState, TScore>(game, start, depth);
        var search = new DepthLimitedSearch<
            TGame, TEvaluation, TOrdering, AlphaBetaCutoff<TScore>,
            TranspositionTable<TState, TScore>, TState, TScore>(game, evaluation, ordering, new());
        (int Move, TScore Value) best = default;

        for (var pass = 1; pass <= depth; pass++)
        {
            best = search.BestMove(start, pass);
        }

        return best;
    }

    // A move can be chosen only to a depth of a ply or more, and only while the game goes on.
    private static void CheckMoveSearch<TGame, TState, TScore>(TGame game, TState start, int depth)
        where TGame : struct, IAlternatingGame<TState, TScore>
        where TScore : INumber<TScore>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(depth, 1);

        if (game.MoveCount(start) == 0)
        {
            throw new ArgumentException(GameOver, nameof(start));
        }
    }
}
