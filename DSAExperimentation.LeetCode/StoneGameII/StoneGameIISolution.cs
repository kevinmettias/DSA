using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.StoneGameII;

// LeetCode 1140. Stone Game II: the player to move from piles[index:] with window
// bound M gets suffixSum[index] minus whatever the opponent can force from the state
// that follows - the same minimax recurrence StoneGameSolution (LC 877) and
// PredictTheWinnerSolution (LC 486) run, keyed on (Index, M) instead of (Left, Right)
// and with an extra loop over how many piles (X in [1, 2M]) to take this turn.
//
// Both strategies precompute the same suffix-sum array - plain array arithmetic, not
// a repo primitive - so the only thing that differs between them is whether the
// (Index, M) states are cached.
internal static class StoneGameIISolution
{
    // LC1140's rule: a turn may take between 1 and 2*M piles.
    private const int MaxTakeMultiplier = 2;

    // The window bound M every game starts from.
    private const int InitialWindow = 1;

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

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed on the
    // exact (Index, M) pair the recurrence branches on, collapsing the exponential
    // recursion to one evaluation per reachable state.
    public static int MaxStonesByMemoizedRecursion(int[] piles)
    {
        var pileCount = piles.Length;
        var suffixSum = SuffixSums(piles);

        return Memoizer.Memoize<(int Index, int Window), int>(
            (0, InitialWindow),
            (state, best) => BestMemoized(pileCount, suffixSum, state, best));
    }

    private static int BestMemoized(
        int pileCount,
        int[] suffixSum,
        (int Index, int Window) state,
        Func<(int Index, int Window), int> best)
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
            var opponent = best((index + take, nextWindow));
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
}
