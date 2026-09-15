using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.BurstBalloons;

// LeetCode 312. Burst Balloons: interval DP over (left, right) boundary pairs - the
// last balloon burst in a sub-range, not the first, is what makes the choice of k
// independent of every other choice in that range, since left and right survive as
// k's neighbors regardless of the order everything else inside them bursts.
//
// Both strategies walk the same recurrence over the same PaddedBalloons array; they
// differ only in whether repeated (left, right) sub-ranges are cached.
internal static class BurstBalloonsSolution
{
    // The textbook answer: plain exponential recursion over (left, right) pairs, no
    // caching - the same (left, right) sub-range recurs across many different choices
    // of which balloon bursts last outside it. Deliberately written without this
    // repo's primitives; it is the arm the composed solution below has to justify
    // itself against.
    public static int MaxCoinsByUnmemoizedRecursion(int[] nums) =>
        MaxCoinsByUnmemoizedRecursion(PaddedBalloons.FromNums(nums));

    public static int MaxCoinsByUnmemoizedRecursion(PaddedBalloons padded) =>
        CoinsBetweenUnmemoized(0, padded.Values.Length - 1, padded.Values);

    private static int CoinsBetweenUnmemoized(int left, int right, int[] values)
    {
        if (right - left <= 1)
        {
            return 0;
        }

        var best = 0;

        for (var last = left + 1; last < right; last++)
        {
            var gained = (values[left] * values[last] * values[right])
                + CoinsBetweenUnmemoized(left, last, values) + CoinsBetweenUnmemoized(last, right, values);
            best = Math.Max(best, gained);
        }

        return best;
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the
    // (left, right) boundary pair - the same 2-tuple-state shape EditDistanceBenchmarks
    // uses.
    public static int MaxCoinsByMemoizedRecursion(int[] nums) =>
        MaxCoinsByMemoizedRecursion(PaddedBalloons.FromNums(nums));

    public static int MaxCoinsByMemoizedRecursion(PaddedBalloons padded) =>
        Memoizer.Memoize<(int Left, int Right), int>(
            (0, padded.Values.Length - 1),
            new CoinsBetweenBoundaries(padded.Values));

    // The recurrence, named: the coins a range yields once its own last balloon is the
    // one that bursts, which is what makes the choice of `last` independent of every
    // other choice in the range. The padded values are the whole of what the rule needs
    // from its caller, so they are the constructor's only input.
    private sealed class CoinsBetweenBoundaries(int[] values) : IRecurrence<(int Left, int Right), int>
    {
        public int Replay((int Left, int Right) range, IRecurrence<(int Left, int Right), int> rest)
        {
            var (left, right) = range;

            if (right - left <= 1)
            {
                return 0;
            }

            var best = 0;

            for (var last = left + 1; last < right; last++)
            {
                var gained = (values[left] * values[last] * values[right])
                    + rest.Replay((left, last), rest) + rest.Replay((last, right), rest);
                best = Math.Max(best, gained);
            }

            return best;
        }
    }
}
