using System.Numerics;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch;

// Negamax to a fixed depth: a position is worth the best, over its moves, of the move's gain less
// the opponent's value after it, as in Negamax; a finished position is worth what its rules say,
// and a position at the depth limit what the static evaluation estimates. A position's value
// depends on the depth left as well as on the position, and a game too large to value every
// position of is the reason to stop at a depth at all, so nothing is memoized the way Negamax
// memoizes; a transposition table may remember positions instead.
//
// Each position is searched in a window (SearchWindow). Once the best found reaches beta the
// opponent would never let the position arise, and the cutoff rule decides whether to stop:
// alpha-beta stops (Knuth and Moore, 1975, showed the value is the same), plain negamax looks at
// every move. A value is returned as found (fail-soft): exact inside the window, and a bound on
// the side it fails.
//
// The table remembers each search's value, what it bounds and its best move. A remembered value
// answers only a search to the same depth, so every configuration finds the value plain negamax
// finds at that depth; the best move is tried first at any depth, which is what makes iterative
// deepening pay: each pass orders the next.
internal sealed class DepthLimitedSearch<
    TGame, TEvaluation, TOrdering, TCutoff, TTable, TState, TScore>(
    TGame game, TEvaluation evaluation, TOrdering ordering, TTable table)
    where TGame : struct, IAlternatingGame<TState, TScore>
    where TEvaluation : struct, IStaticEvaluation<TState, TScore>
    where TOrdering : struct, IMoveOrdering<TState>
    where TCutoff : struct, ICutoffRule<TScore>
    where TTable : struct, ITranspositionTable<TState, TScore>
    where TScore : INumber<TScore>, IMinMaxValue<TScore>
{
    // Positions with this many moves or fewer order them in a stack buffer.
    private const int StackMoveLimit = 64;

    // A move number no move has: no remembered move to try first.
    private const int NoMove = -1;

    // A position's value with depth plies left, searched in the widest window, so exact.
    public TScore ValueOf(TState state, int depth) =>
        ValueOf(state, depth, SearchWindow<TScore>.Widest);

    // The best move from a position and its value with depth plies left, searched in the widest
    // window; on a tie the move tried first is kept.
    public (int Move, TScore Value) BestMove(TState state, int depth)
    {
        var moves = new int[game.MoveCount(state)];
        var remembered = RememberedMove(state);
        OrderMoves(state, moves, remembered);
        var best = BestOf(state, moves, depth, SearchWindow<TScore>.Widest);
        table.Remember(state, new(depth, best.Value, ScoreBound.Exact, best.Move));

        return best;
    }

    // A position's value with depth plies left, searched in the window.
    private TScore ValueOf(TState state, int depth, SearchWindow<TScore> window)
    {
        var moveCount = game.MoveCount(state);

        if (moveCount == 0 || depth <= 0)
        {
            return UnsearchedValue(state, moveCount);
        }

        var isRemembered = table.TryRecall(state, out var entry);

        if (isRemembered && entry.CanAnswer(depth, window))
        {
            return entry.Value;
        }

        var moves = moveCount <= StackMoveLimit ? stackalloc int[moveCount] : new int[moveCount];
        var remembered = isRemembered ? entry.Move : NoMove;
        OrderMoves(state, moves, remembered);
        var best = BestOf(state, moves, depth, window);
        table.Remember(state, new(depth, best.Value, window.BoundOf(best.Value), best.Move));

        return best.Value;
    }

    // What a position the search does not look past is worth: what its rules say when the game is
    // over, and the evaluation's estimate at the depth limit.
    private TScore UnsearchedValue(TState state, int moveCount)
    {
        if (moveCount == 0)
        {
            return game.ValueWhenOver(state);
        }

        return evaluation.Evaluate(state);
    }

    private int RememberedMove(TState state)
    {
        if (table.TryRecall(state, out var entry))
        {
            return entry.Move;
        }

        return NoMove;
    }

    // The ordering's moves, with the move the table remembers as best, if any, moved to the front
    // and the rest kept in the ordering's order.
    private void OrderMoves(TState state, Span<int> moves, int remembered)
    {
        ordering.Order(state, moves);

        if (remembered == NoMove)
        {
            return;
        }

        for (var at = moves.IndexOf(remembered); at > 0; at--)
        {
            moves[at] = moves[at - 1];
            moves[at - 1] = remembered;
        }
    }

    // The best of the ordered moves and its value, each searched in the window as the moves before
    // it have raised it, stopping where the cutoff rule says; of moves worth the same, the first.
    private (int Move, TScore Value) BestOf(
        TState state, ReadOnlySpan<int> moves, int depth, SearchWindow<TScore> window)
    {
        var bestMove = moves[0];
        var best = MoveValue(state, bestMove, depth, window);

        for (var rank = 1; rank < moves.Length; rank++)
        {
            window = window.RaisedTo(best);

            if (TCutoff.IsCutoff(window.Alpha, window.Beta))
            {
                break;
            }

            var value = MoveValue(state, moves[rank], depth, window);

            if (value > best)
            {
                (bestMove, best) = (moves[rank], value);
            }
        }

        return (bestMove, best);
    }

    // A move's worth to its mover: its gain less the reply's value, the reply searched one ply
    // shallower in the window seen from the opponent's side.
    private TScore MoveValue(TState state, int move, int depth, SearchWindow<TScore> window)
    {
        var gain = game.Gain(state, move);
        var after = game.Next(state, move);
        var reply = window.ForReply(gain);

        return gain - ValueOf(after, depth - 1, reply);
    }
}
