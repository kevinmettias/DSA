using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.GuessNumberHigherOrLowerII;

// LeetCode 375. Guess Number Higher or Lower II: minimax interval DP over (low, high)
// bounds - for each range, the guesser picks k to minimize the worst-case money the
// adversary can force by revealing the wrong half, and that worst case is itself the
// max of the two sub-range costs.
//
// Both strategies walk the same recurrence over (low, high) bounds; they differ only
// in whether repeated sub-ranges are cached.
internal static class GuessNumberHigherOrLowerIISolution
{
    // The textbook answer: plain exponential recursion over (low, high) bounds, no
    // caching - the same (low, high) sub-range recurs across many different choices
    // of guess outside it. Deliberately written without this repo's primitives; it is
    // the arm the memoized strategy below has to justify itself against.
    public static int GetMoneyAmountByUnmemoizedRecursion(int highestNumber) =>
        WorstCaseCostUnmemoized(1, highestNumber);

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the
    // (low, high) bound pair - the same 2-tuple-state shape BurstBalloons already
    // uses for its own interval DP.
    public static int GetMoneyAmountByMemoizedRecursion(int highestNumber) =>
        Memoizer.Memoize<(int Low, int High), int>((1, highestNumber), new WorstCaseGuessCost());

    /// <summary>
    /// The recurrence, named: over a (low, high) range the guesser picks a guess
    /// minimizing the worst-case money the adversary can force by revealing the wrong
    /// half, and that worst case is itself the larger of the two sub-range costs.
    /// </summary>
    private sealed class WorstCaseGuessCost : IRecurrence<(int Low, int High), int>
    {
        /// <inheritdoc/>
        public int Replay((int Low, int High) state, IRecurrence<(int Low, int High), int> rest)
        {
            var (low, high) = state;

            if (low >= high)
            {
                return 0;
            }

            var best = int.MaxValue;

            for (var guess = low; guess <= high; guess++)
            {
                var lowHalf = rest.Replay((low, guess - 1), rest);
                var highHalf = rest.Replay((guess + 1, high), rest);
                var worstHalf = Math.Max(lowHalf, highHalf);
                best = Math.Min(best, guess + worstHalf);
            }

            return best;
        }
    }

    private static int WorstCaseCostUnmemoized(int low, int high)
    {
        if (low >= high)
        {
            return 0;
        }

        var best = int.MaxValue;

        for (var guess = low; guess <= high; guess++)
        {
            var lowHalf = WorstCaseCostUnmemoized(low, guess - 1);
            var highHalf = WorstCaseCostUnmemoized(guess + 1, high);
            best = Math.Min(best, guess + Math.Max(lowHalf, highHalf));
        }

        return best;
    }
}
