using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.PartitionArrayIntoTwoEqualProductSubsets;

// LeetCode 3566. Partition Array into Two Equal Product Subsets: split nums into
// two non-empty groups, every element in exactly one, so that each group's product
// equals target.
//
// Both strategies assign every index to one of two groups and stop as soon as a
// full assignment satisfies both products - the only difference is whether a
// partial assignment is allowed to keep growing once it can no longer possibly
// reach target. Every nums[i] >= 1, so a group's running product never decreases,
// which is what makes that pruning sound.
internal static class PartitionArrayIntoTwoEqualProductSubsetsSolution
{
    // Every one of the 2^n ways to split the indices between the two groups,
    // tried directly as a bitmask. A running product that exceeds target aborts
    // that mask early - required for correctness, not just speed, since nums[i] <=
    // 100 and n <= 12 can otherwise overflow long well before target's 10^15 cap.
    // The arm the pruned backtracking search below has to beat.
    public static bool CheckEqualPartitionsByBitmaskEnumeration(int[] nums, long target)
    {
        var n = nums.Length;
        var fullMask = (1 << n) - 1;

        for (var mask = 1; mask < fullMask; mask++)
        {
            if (TryProduct(nums, mask, target, out var group1) &&
                TryProduct(nums, fullMask & ~mask, target, out var group2) &&
                group1 == target && group2 == target)
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryProduct(int[] nums, int mask, long target, out long product)
    {
        product = 1;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) == 0)
            {
                continue;
            }

            product *= nums[i];

            if (product > target)
            {
                return false;
            }
        }

        return true;
    }

    // This repo's own Backtrack.TrySearch, choosing group 1 or group 2 for each
    // index in turn: Candidates only offers an assignment whose resulting running
    // product would stay within target, so a branch that has already overshot is
    // never explored at all rather than discovered at the leaf.
    public static bool CheckEqualPartitionsByPrunedBacktracking(int[] nums, long target)
    {
        var state = new PartitionState();

        return Backtrack.TrySearch(state, new BacktrackingSteps<PartitionState, bool>(
            IsSolution: s => s.Index == nums.Length,
            Candidates: s => s.Index == nums.Length
                ? Array.Empty<bool>()
                : CandidateAssignments(s, nums, target),
            Choose: (s, toGroup1) => Choose(s, nums, toGroup1),
            Unchoose: (s, toGroup1) => Unchoose(s, nums, toGroup1),
            OnSolution: s =>
                s.Count1 > 0 && s.Count1 < nums.Length && s.Product1 == target && s.Product2 == target));
    }

    private static IEnumerable<bool> CandidateAssignments(PartitionState state, int[] nums, long target)
    {
        var value = nums[state.Index];

        if (state.Product1 * value <= target)
        {
            yield return true;
        }

        if (state.Product2 * value <= target)
        {
            yield return false;
        }
    }

    private static void Choose(PartitionState state, int[] nums, bool toGroup1)
    {
        var value = nums[state.Index];

        if (toGroup1)
        {
            state.Product1 *= value;
            state.Count1++;
        }
        else
        {
            state.Product2 *= value;
        }

        state.Index++;
    }

    private static void Unchoose(PartitionState state, int[] nums, bool toGroup1)
    {
        state.Index--;
        var value = nums[state.Index];

        if (toGroup1)
        {
            state.Product1 /= value;
            state.Count1--;
        }
        else
        {
            state.Product2 /= value;
        }
    }

    private sealed class PartitionState
    {
        public long Product1 { get; set; } = 1;

        public long Product2 { get; set; } = 1;

        public int Count1 { get; set; }

        public int Index { get; set; }
    }
}
