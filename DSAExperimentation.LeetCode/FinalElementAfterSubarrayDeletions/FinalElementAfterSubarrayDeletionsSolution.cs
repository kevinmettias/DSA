using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.FinalElementAfterSubarrayDeletions;

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
internal static class FinalElementAfterSubarrayDeletionsSolution
{
    public static int FinalElementByDictionaryMinimax(int[] nums)
    {
        var fullMask = (1 << nums.Length) - 1;
        var memo = new Dictionary<(int Mask, bool AliceTurn), int>();

        int Recurse((int Mask, bool AliceTurn) state)
        {
            if (memo.TryGetValue(state, out var cached))
            {
                return cached;
            }

            var result = PlayStep(state, nums, Recurse);
            memo[state] = result;
            return result;
        }

        return Recurse((fullMask, true));
    }

    public static int FinalElementByMemoizedMinimax(int[] nums)
    {
        var fullMask = (1 << nums.Length) - 1;

        int Recurrence((int Mask, bool AliceTurn) state, Func<(int, bool), int> solveRest) =>
            PlayStep(state, nums, solveRest);

        return Memoizer.Memoize<(int Mask, bool AliceTurn), int>((fullMask, true), Recurrence);
    }

    public static int FinalElementByEndpointComparison(int[] nums) =>
        Math.Max(nums[0], nums[^1]);

    private static int PlayStep(
        (int Mask, bool AliceTurn) state,
        int[] nums,
        Func<(int Mask, bool AliceTurn), int> solveRest)
    {
        var (mask, aliceTurn) = state;
        var survivors = SurvivorPositions(mask);
        var count = survivors.Count;

        if (count == 1)
        {
            return nums[survivors[0]];
        }

        var best = aliceTurn ? int.MinValue : int.MaxValue;

        for (var start = 0; start < count; start++)
        {
            var removedMask = 0;

            for (var end = start; end < count && end - start + 1 < count; end++)
            {
                removedMask |= 1 << survivors[end];
                var value = solveRest((mask & ~removedMask, !aliceTurn));
                best = aliceTurn ? Math.Max(best, value) : Math.Min(best, value);
            }
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
}
