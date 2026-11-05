using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.LeetCode.Solutions.StoneGameII;

// LeetCode 1140. Stone Game II: the player to move from piles[index:] with window
// bound M gets suffixSum[index] minus whatever the opponent can force from the state
// that follows - the same minimax recurrence StoneGameSolution (LC 877) and
// PredictTheWinnerSolution (LC 486) run, keyed on (Index, M) instead of (Left, Right)
// and with an extra loop over how many piles (X in [1, 2M]) to take this turn.
//
// Both strategies precompute the same suffix-sum array - plain array arithmetic, not
// a repo primitive - so the only thing that differs between them is whether the
// (Index, M) states are cached.
[LeetCodeQuestion("stone-game-ii", LeetCodeAnswerOrder.AsWritten, Reach = typeof(StoneGameIIReach))]
internal static class StoneGameIISolution
{
    // LC1140's rule: a turn may take between 1 and 2*M piles.
    private const int MaxTakeMultiplier = 2;

    // The window bound M every game starts from.
    private const int InitialWindow = 1;

    // Alice's stones and Bob's add up to the row and differ by what Alice can force, so hers are
    // half of the row plus that difference.
    private const int Halves = 2;

    // The textbook baseline: plain minimax recursion over (index, M) with no
    // caching, so the same state recurs through every pick-sequence that reaches it
    // and the cost is exponential. Deliberately written without this repo's
    // primitives - it is the arm the memoized strategy has to justify itself
    // against.
    public static int MaxStonesByUnmemoizedRecursion(int[] piles)
    {
        var suffixSum = SuffixSums(piles);

        return Best(piles.Length, suffixSum, InitialWindow, 0);
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed on the
    // exact (Index, M) pair the recurrence branches on, collapsing the exponential
    // recursion to one evaluation per reachable state.
    public static int MaxStonesByMemoizedRecursion(int[] piles)
    {
        var pileCount = piles.Length;
        var suffixSum = SuffixSums(piles);

        return Memoizer.Memoize<(int Index, int Window), int>(
            (0, InitialWindow),
            new WindowPickOrder(pileCount, suffixSum));
    }

    // The rule named as Domain's alternating game, valued by Algorithms/GameSearch's negamax: the
    // recursion above, scoring each turn's piles rather than the rest of the row, with the minimax
    // and its memo the library's.
    public static int MaxStonesByNegamax(int[] piles)
    {
        var suffixSum = SuffixSums(piles);
        var difference = Negamax.Value<WindowTakeGame, (int Index, int Window), int>(
            new WindowTakeGame(piles.Length, suffixSum), (0, InitialWindow));

        return (suffixSum[0] + difference) / Halves;
    }

    // The rule, named: from (index, window) the player to move takes some X in [1, 2M]
    // and scores the rest of the board minus whatever the opponent can then force from
    // the wider window that leaves behind, keeping the best X. The board's suffix sums
    // and its pile count are the whole of what the rule needs from its caller, so they
    // are the constructor's only inputs.
    private sealed class WindowPickOrder(int pileCount, int[] suffixSum)
        : IRecurrence<(int Index, int Window), int>
    {
        public int Replay((int Index, int Window) state, IRecurrence<(int Index, int Window), int> rest)
        {
            var (index, window) = state;

            if (index + (MaxTakeMultiplier * window) >= pileCount)
            {
                return suffixSum[index];
            }

            var result = 0;

            for (var take = 1; take <= MaxTakeMultiplier * window; take++)
            {
                var nextWindow = Math.Max(window, take);
                var opponent = rest.Replay((index + take, nextWindow), rest);
                result = Math.Max(result, suffixSum[index] - opponent);
            }

            return result;
        }
    }

    private static int Best(int pileCount, int[] suffixSum, int window, int index)
    {
        if (index + (MaxTakeMultiplier * window) >= pileCount)
        {
            return suffixSum[index];
        }

        var result = 0;

        for (var take = 1; take <= MaxTakeMultiplier * window; take++)
        {
            var nextWindow = Math.Max(window, take);
            var opponent = Best(pileCount, suffixSum, nextWindow, index + take);
            result = Math.Max(result, suffixSum[index] - opponent);
        }

        return result;
    }

    // suffixSum[i] is the total of piles[i..], so a state's whole remaining board is
    // one lookup and the recurrence only has to decide how it splits.
    private static int[] SuffixSums(int[] piles)
    {
        var suffixSum = new int[piles.Length + 1];

        for (var i = piles.Length - 1; i >= 0; i--)
        {
            suffixSum[i] = suffixSum[i + 1] + piles[i];
        }

        return suffixSum;
    }

    // The rule as an alternating game: from (index, window) a turn takes the next 1 to 2M piles,
    // move i taking i + 1 of them and widening the window to at least that many, and scores the
    // piles it took; nothing is left to score once the row is taken.
    private readonly struct WindowTakeGame(int pileCount, int[] suffixSum)
        : IAlternatingGame<(int Index, int Window), int>
    {
        public int MoveCount((int Index, int Window) state) =>
            Math.Min(MaxTakeMultiplier * state.Window, pileCount - state.Index);

        public (int Index, int Window) Next((int Index, int Window) state, int move)
        {
            var take = move + 1;

            return (state.Index + take, Math.Max(state.Window, take));
        }

        public int Gain((int Index, int Window) state, int move) =>
            suffixSum[state.Index] - suffixSum[state.Index + move + 1];

        public int ValueWhenOver((int Index, int Window) state) => 0;
    }
}
