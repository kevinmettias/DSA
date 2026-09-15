using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfWaysToRearrangeSticksWithKSticksVisible;

// LeetCode 1866. Number of Ways to Rearrange Sticks With K Sticks Visible: how many
// arrangements of sticks [1..n] leave exactly k of them visible from the left, where
// a stick is visible iff it is taller than every stick before it.
//
// Both strategies answer the same count, reported mod 1e9+7 the way LeetCode asks.
// RearrangeSticksByPermutationEnumeration reads the definition literally - lay out
// every one of the n! arrangements and count the ones with k visible sticks.
// RearrangeSticksByMemoizedStirling is the unsigned Stirling numbers of the first
// kind, T(n,k) = T(n-1,k-1) + (n-1)*T(n-1,k): stick n is the tallest, so it either
// stands as its own new visible stick (T(n-1,k-1), one way) or is slotted in behind
// one of the n-1 already-arranged sticks where it stays hidden (T(n-1,k) ways, times
// n-1 insertion slots). Routing that recurrence through this repo's own
// Memoizer<TState,TResult>, keyed on the (sticks, visible) pair, solves each state
// once - the same top-down shape UniqueBinarySearchTrees uses for a different
// counting recurrence.
internal static class NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
{
    // The textbook answer: enumerate every permutation of [1..stickCount] by
    // swapping in place and count the ones whose left-to-right maxima number exactly
    // visibleCount. Deliberately BCL-only - it is the O(n!) arm the recurrence below
    // has to justify itself against.
    public static int RearrangeSticksByPermutationEnumeration(int stickCount, int visibleCount)
    {
        var sticks = new int[stickCount];

        for (var i = 0; i < stickCount; i++)
        {
            sticks[i] = i + 1;
        }

        var arrangements = 0L;
        CountArrangements(sticks, 0, visibleCount, ref arrangements);

        return (int)(arrangements % ModularArithmetic.Modulo);
    }

    // This repo's own Memoizer over the (remaining sticks, remaining visible) state,
    // so the stickCount*visibleCount distinct states are each solved once instead of
    // once per path that reaches them.
    public static int RearrangeSticksByMemoizedStirling(int stickCount, int visibleCount)
        => (int)Memoizer.Memoize<(int Sticks, int Visible), long>((stickCount, visibleCount), new StirlingWays());

    // One step of the Stirling recurrence, named. No sticks left is an arrangement
    // exactly when no visible sticks are still owed; sticks left with none owed
    // cannot happen, since the tallest remaining stick is always visible from
    // somewhere.
    private sealed class StirlingWays : IRecurrence<(int Sticks, int Visible), long>
    {
        public long Replay((int Sticks, int Visible) state, IRecurrence<(int Sticks, int Visible), long> rest)
        {
            var (sticks, visible) = state;

            if (sticks == 0)
            {
                return visible == 0 ? 1 : 0;
            }

            if (visible == 0)
            {
                return 0;
            }

            var placeAsNewVisible = rest.Replay((sticks - 1, visible - 1), rest);
            var hideAfterExistingStick =
                (sticks - 1) * rest.Replay((sticks - 1, visible), rest) % ModularArithmetic.Modulo;

            return (placeAsNewVisible + hideAfterExistingStick) % ModularArithmetic.Modulo;
        }
    }

    private static void CountArrangements(int[] sticks, int index, int visibleCount, ref long arrangements)
    {
        if (index == sticks.Length)
        {
            if (VisibleSticks(sticks) == visibleCount)
            {
                arrangements++;
            }

            return;
        }

        for (var i = index; i < sticks.Length; i++)
        {
            (sticks[index], sticks[i]) = (sticks[i], sticks[index]);
            CountArrangements(sticks, index + 1, visibleCount, ref arrangements);
            (sticks[index], sticks[i]) = (sticks[i], sticks[index]);
        }
    }

    // A stick is visible exactly when it is a left-to-right maximum.
    private static int VisibleSticks(int[] sticks)
    {
        var visible = 0;
        var tallestSoFar = 0;

        foreach (var stick in sticks)
        {
            if (stick > tallestSoFar)
            {
                visible++;
                tallestSoFar = stick;
            }
        }

        return visible;
    }
}
