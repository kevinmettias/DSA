using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Solutions.FinalElementAfterSubarrayDeletions;

// LeetCode 3828. Final Element After Subarray Deletions: Alice and Bob
// alternately delete a subarray shorter than the current array (Alice
// first), until one element remains; Alice maximizes it, Bob minimizes it,
// both playing optimally.
//
// FinalElementByDictionaryMinimax/FinalElementByMemoizedMinimax both play the
// game out in full: state is (Mask, AliceTurn), Mask a bitmask of the
// original indices still alive. A move removes any contiguous run of the
// *survivors* (their positions among what remains, not their original
// indices - earlier removals can leave gaps) shorter than the survivor
// count, exactly the rule as stated, just replayed literally instead of
// solved. They differ only in how repeated states get cached, the same
// Dictionary-vs-Memoizer contrast MinimumSumOfValuesByDividingArraySolution
// draws - and only handle small inputs, since the state space is every
// surviving subset of the original array.
//
// FinalElementByEndpointComparison is the closed form the exhaustive
// strategies above exist to confirm: Alice can always spend her turns
// emptying everything strictly between the two ends, so she banks at least
// max(nums[0], nums[^1]); Bob can always answer by targeting whichever
// interior element Alice tries to preserve instead, so neither player ever
// does better than that bound. O(1), no repo primitive needed.
[LeetCodeQuestion("final-element-after-subarray-deletions", LeetCodeAnswerOrder.AsWritten)]
internal static class FinalElementAfterSubarrayDeletionsSolution
{
    public static int FinalElementByDictionaryMinimax(int[] nums)
    {
        var fullMask = (1 << nums.Length) - 1;
        var memo = new Dictionary<(int Mask, bool AliceTurn), int>();

        return DictionaryMinimax((fullMask, true), nums, memo);
    }

    public static int FinalElementByMemoizedMinimax(int[] nums)
    {
        var fullMask = (1 << nums.Length) - 1;

        return Memoizer.Memoize<(int Mask, bool AliceTurn), int>((fullMask, true), new MinimaxFromState(nums));
    }

    public static int FinalElementByEndpointComparison(int[] nums) =>
        Math.Max(nums[0], nums[^1]);

    private static int PlayStep(
        (int Mask, bool AliceTurn) state,
        int[] nums,
        IRecurrence<(int Mask, bool AliceTurn), int> solveRest)
    {
        var survivors = SurvivorPositions(state.Mask);

        if (survivors.Count == 1)
        {
            return nums[survivors[0]];
        }

        var aliceTurn = state.AliceTurn;
        var best = aliceTurn ? int.MinValue : int.MaxValue;

        foreach (var next in NextStates(state, survivors))
        {
            var value = solveRest.Replay(next, solveRest);
            best = aliceTurn ? Math.Max(best, value) : Math.Min(best, value);
        }

        return best;
    }

    // The hand-rolled arm: plain recursion over the caller's own Dictionary, the cache the
    // memoized arm hands to Memoizer instead - written out rather than through IRecurrence,
    // since a baseline uses no library implementation (ARCHITECTURE 17.5).
    private static int DictionaryMinimax(
        (int Mask, bool AliceTurn) state, int[] nums, Dictionary<(int Mask, bool AliceTurn), int> memo)
    {
        if (memo.TryGetValue(state, out var cached))
        {
            return cached;
        }

        var survivors = SurvivorPositions(state.Mask);
        var result = nums[survivors[0]];

        if (survivors.Count > 1)
        {
            result = BestDictionaryMove(state, survivors, nums, memo);
        }

        memo[state] = result;

        return result;
    }

    // The best value whoever is to play can force from here, each reply answered by
    // DictionaryMinimax's own recursion.
    private static int BestDictionaryMove(
        (int Mask, bool AliceTurn) state,
        List<int> survivors,
        int[] nums,
        Dictionary<(int Mask, bool AliceTurn), int> memo)
    {
        var best = state.AliceTurn ? int.MinValue : int.MaxValue;

        foreach (var next in NextStates(state, survivors))
        {
            var value = DictionaryMinimax(next, nums, memo);
            best = state.AliceTurn ? Math.Max(best, value) : Math.Min(best, value);
        }

        return best;
    }

    private static List<int> SurvivorPositions(int mask)
    {
        var positions = new List<int>();

        while (mask != 0)
        {
            positions.Add(BitOperations.TrailingZeroCount(mask));
            mask &= mask - 1;
        }

        return positions;
    }

    // Every move available to whoever is to play: delete a contiguous run of the
    // survivors (positions among what remains, not original indices) shorter than
    // the survivor count. Yields each resulting state in the same order the nested
    // start/end scan would reach it, so the caller's recursion runs exactly where
    // it did before.
    private static IEnumerable<(int Mask, bool AliceTurn)> NextStates(
        (int Mask, bool AliceTurn) state,
        List<int> survivors)
    {
        var (mask, aliceTurn) = state;
        var count = survivors.Count;

        for (var start = 0; start < count; start++)
        {
            var removedMask = 0;

            for (var end = start; end < count && end - start + 1 < count; end++)
            {
                removedMask |= 1 << survivors[end];
                yield return (mask & ~removedMask, !aliceTurn);
            }
        }
    }

    // The recurrence, as a named type: a position's value is PlayStep's best over the
    // positions one move reaches - the rule the memoized arm plays the game out through,
    // and the one DictionaryMinimax restates.
    private sealed class MinimaxFromState(int[] nums) : IRecurrence<(int Mask, bool AliceTurn), int>
    {
        public int Replay((int Mask, bool AliceTurn) state, IRecurrence<(int Mask, bool AliceTurn), int> rest) =>
            PlayStep(state, nums, rest);
    }
}
