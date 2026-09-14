using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.StoneGameVIII;

// LeetCode 1872. Stone Game VIII: each move removes the first x (x > 1) stones and
// puts one stone worth their sum back at the front, so the score of ANY move is
// simply the prefix sum up to whichever boundary index it ends at - and because the
// replacement stone always counts toward the next move's x >= 2, boundaries only
// need to strictly increase. That collapses the whole game into a single backward
// chain over boundary indices, one state depending only on the next:
// best(i) = max(best(i + 1), prefix[i] - best(i + 1)) - "end my move at this
// boundary now, then subtract whatever the opponent forces from the rest" vs. "defer
// to a later boundary, whose already-computed best carries forward unchanged".
//
// The two strategies are that same recurrence with and without a cache. The chain is
// one-dimensional, so the unmemoized arm's blowup is the pure 2^n of re-walking
// best(i + 1) twice at every boundary - the same shape StoneGameVIISolution's
// baseline has over its (left, right) interval, here over a single index.
internal static class StoneGameVIIISolution
{
    // LeetCode guarantees stones.Length >= 2, and the first move must remove x > 1
    // stones - so the earliest boundary anyone may end a move at is index 1 (taking
    // stones[0] and stones[1]), never 0.
    private const int FirstBoundary = 1;

    // The textbook baseline: plain minimax recursion with no caching, so every
    // boundary re-explores the entire tail behind it once per branch. Deliberately a
    // bare recursive walk over the prefix table with nothing from this repo in its
    // internals - it is the arm the memoized strategy has to justify itself against,
    // and stating it here is what finally gets it asserted.
    public static long MaxScoreDifferenceByUnmemoizedRecursion(int[] stones) =>
        MaxScoreDifferenceByUnmemoizedRecursion(BuildPrefixSums(stones));

    public static long MaxScoreDifferenceByUnmemoizedRecursion(ArraySequence<long> prefix) =>
        Best(prefix, FirstBoundary);

    private static long Best(ArraySequence<long> prefix, int boundary)
    {
        // The last boundary takes every remaining stone, so there is no later
        // boundary left to defer to and no opponent move to subtract.
        if (boundary == prefix.Length - 1)
        {
            return prefix.Get(boundary);
        }

        // Both branches ask for the same next boundary and this arm has no cache, so
        // it descends the whole tail twice. That is deliberate: the double descent IS
        // the 2^n blowup the memoized strategy exists to remove, and collapsing these
        // two calls into one local would quietly turn the baseline linear.
        var takeHere = prefix.Get(boundary) - Best(prefix, boundary + 1);
        var deferToLater = Best(prefix, boundary + 1);

        return Math.Max(deferToLater, takeHere);
    }

    // The same recurrence routed through this repo's own Memoizer<TState,TResult>,
    // keyed on the single boundary index the recurrence branches on, so each of the
    // n boundaries is evaluated once instead of once per path that reaches it - the
    // identical top-down memoization StoneGameVIISolution uses for LC 1690, with a
    // single-index state in place of its (left, right) pair.
    public static long MaxScoreDifferenceByMemoizedRecursion(int[] stones) =>
        MaxScoreDifferenceByMemoizedRecursion(BuildPrefixSums(stones));

    public static long MaxScoreDifferenceByMemoizedRecursion(ArraySequence<long> prefix) =>
        Memoizer.Memoize<int, long>(
            FirstBoundary, (boundary, best) => BestMemoized(prefix, boundary, best));

    private static long BestMemoized(ArraySequence<long> prefix, int boundary, Func<int, long> best)
    {
        if (boundary == prefix.Length - 1)
        {
            return prefix.Get(boundary);
        }

        // The same two asks the un-memoized arm makes, so the two arms compare
        // like for like - the difference being that here the second one is a cache
        // hit rather than another descent.
        var takeHere = prefix.Get(boundary) - best(boundary + 1);
        var deferToLater = best(boundary + 1);

        return Math.Max(deferToLater, takeHere);
    }

    // prefix[i] is the value of the single stone the board collapses to when a move
    // ends at boundary i, which is exactly the score that move earns. Public so a
    // benchmark can charge this O(n) pass to [GlobalSetup] and hand the prepared
    // table to the overloads above, rather than restating the table's own
    // construction in the harness.
    public static ArraySequence<long> BuildPrefixSums(int[] stones)
    {
        var prefix = new long[stones.Length];
        prefix[0] = stones[0];

        for (var i = 1; i < stones.Length; i++)
        {
            prefix[i] = prefix[i - 1] + stones[i];
        }

        return new ArraySequence<long>(prefix);
    }
}
