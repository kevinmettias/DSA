using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.FindNumberOfWaysToReachTheKthStair;

// LeetCode 3154. Find Number of Ways to Reach the K-th Stair: Alice starts on
// stair 1 with jump = 0 and counts every distinct operation sequence that ever
// lands her on stair k, where she may go down one stair (never twice in a row,
// never from stair 0) or up stair + 2^jump (then jump increments). Once the
// current stair exceeds k + 1, no further sequence of moves can ever land back
// on k - a single down nets -1 but must be followed by an up that adds at least
// 2^0 = 1, so the position never decreases below where it already overshot -
// which is what bounds both strategies below to O(log k) recursion depth
// despite k running up to 1e9.
internal static class FindNumberOfWaysToReachTheKthStairSolution
{
    // The textbook answer: plain unmemoized recursion over Alice's own two
    // operations. Every (Stair, Jump, CanStepDown) triple gets recomputed once
    // per distinct interleaving of up/down moves that reaches it - O(k) calls,
    // the baseline the memoized arm below has to beat.
    public static int WaysByBruteRecursion(int k) => CountWays(1, 0, DownStep.Available, k);

    // This repo's own Memoizer over the identical recurrence - the same
    // (Stair, Jump, CanStepDown) triple is reached by many distinct
    // interleavings of up/down moves once the k+1 prune stops firing, so
    // caching collapses those into one evaluation each, the same DP
    // composition IntegerReplacementTests uses.
    public static int WaysByMemoizedRecurrence(int k) =>
        Memoizer.Memoize<StairState, int>(new StairState(1, 0, true), new StairWalkTo(k));

    /// <summary>
    /// The recurrence, named: from a (stair, jump, can-step-down) position the count is
    /// 1 when this is stair <paramref name="k"/>, plus the count after the up move, plus
    /// the count after the single down move while it is still available - and 0 once the
    /// stair has overshot k + 1, which is what bounds the depth.
    /// </summary>
    private sealed class StairWalkTo(int k) : IRecurrence<StairState, int>
    {
        /// <inheritdoc/>
        public int Replay(StairState state, IRecurrence<StairState, int> rest)
        {
            if (state.Stair > k + 1)
            {
                return 0;
            }

            var total = state.Stair == k ? 1 : 0;
            total += rest.Replay(state with
            {
                Stair = state.Stair + (1L << state.Jump),
                Jump = state.Jump + 1,
                CanStepDown = true,
            }, rest);

            if (state.CanStepDown && state.Stair > 0)
            {
                total += rest.Replay(state with { Stair = state.Stair - 1, CanStepDown = false }, rest);
            }

            return total;
        }
    }

    private static int CountWays(long stair, int jump, DownStep downStep, int k)
    {
        if (stair > k + 1)
        {
            return 0;
        }

        var ways = stair == k ? 1 : 0;
        ways += CountWays(stair + (1L << jump), jump + 1, DownStep.Available, k);

        if (downStep == DownStep.Available && stair > 0)
        {
            ways += CountWays(stair - 1, jump, DownStep.Spent, k);
        }

        return ways;
    }

    // Whether Alice's down-one-stair operation is still open to her, or was just
    // spent and may not be repeated: named where the rewritten `true`/`false` at
    // the call sites said it only by position. An up move restores Available.
    private enum DownStep
    {
        Available,
        Spent,
    }

    private readonly record struct StairState(long Stair, int Jump, bool CanStepDown);
}
