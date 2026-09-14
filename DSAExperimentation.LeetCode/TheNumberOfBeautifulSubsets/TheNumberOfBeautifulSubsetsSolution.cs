using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.TheNumberOfBeautifulSubsets;

// LeetCode 2597. The Number of Beautiful Subsets: count the non-empty subsets of
// nums that contain no pair of elements differing by exactly k.
//
// Subsets are counted by index, not by distinct value - two equal-valued elements
// at different indices never conflict with each other (|x-x| == 0 != k for the
// k >= 1 this problem guarantees), so duplicates are free to combine, exactly as
// LeetCode's own examples require.
//
// The two strategies differ only in when the conflict rule is applied: after a
// candidate subset has been generated in full, or while it is still being built.
internal static class TheNumberOfBeautifulSubsetsSolution
{
    // The textbook answer: enumerate all 2^n index subsets as bitmasks and reject
    // the ones containing a |x-y| == k pair afterwards, checking all C(size,2)
    // pairs per subset. Deliberately written without this repo's primitives - it
    // is the arm the pruned search below has to justify itself against.
    public static int CountBeautifulSubsetsByBitmask(int[] nums, int k)
    {
        var count = 0;
        var subsetCount = 1 << nums.Length;

        for (var mask = 1; mask < subsetCount; mask++)
        {
            if (IsBeautiful(nums, k, mask))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsBeautiful(int[] nums, int k, int mask)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) == 0)
            {
                continue;
            }

            for (var j = i + 1; j < nums.Length; j++)
            {
                if ((mask & (1 << j)) != 0 && Math.Abs(nums[i] - nums[j]) == k)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // This repo's own Backtrack.Search, deciding include/exclude for each index in
    // turn with the legality rule folded straight into Candidates - the same "prune
    // in Candidates, don't generate-then-filter" shape BeautifulArrangement uses. A
    // HashMap<value,count> tracks how many currently-included elements sit at each
    // value, so "would including nums[index] create a |x-y| == k pair" is an O(1)
    // pair of lookups (value-k, value+k) instead of a scan of the partial subset,
    // and an illegal inclusion is never made rather than discovered after the fact.
    public static int CountBeautifulSubsetsByPrunedBacktracking(int[] nums, int k)
    {
        var count = 0;
        var state = new SubsetWalk();

        Backtrack.Search<SubsetWalk, bool>(
            state,
            s => s.Index == nums.Length,
            s => Candidates(s, nums, k),
            (s, include) => Choose(s, nums, include),
            (s, include) => Unchoose(s, nums, include),
            s =>
            {
                if (s.Size > 0)
                {
                    count++;
                }
            });

        return count;
    }

    private static IEnumerable<bool> Candidates(SubsetWalk state, int[] nums, int k)
    {
        if (state.Index == nums.Length)
        {
            yield break;
        }

        yield return false;

        if (CanInclude(state, nums[state.Index], k))
        {
            yield return true;
        }
    }

    private static bool CanInclude(SubsetWalk state, int value, int k)
    {
        state.Frequency.TryGetValue(value - k, out var lower);
        state.Frequency.TryGetValue(value + k, out var upper);

        return lower == 0 && upper == 0;
    }

    private static void Choose(SubsetWalk state, int[] nums, bool include)
    {
        if (include)
        {
            var value = nums[state.Index];
            state.Frequency.TryGetValue(value, out var current);
            state.Frequency.Set(value, current + 1);
            state.Size++;
        }

        state.Index++;
    }

    private static void Unchoose(SubsetWalk state, int[] nums, bool include)
    {
        state.Index--;

        if (include)
        {
            var value = nums[state.Index];
            state.Frequency.TryGetValue(value, out var current);
            state.Frequency.Set(value, current - 1);
            state.Size--;
        }
    }

    // Backtrack.Search mutates one shared state object rather than snapshotting, so
    // this is a class by requirement, not by preference.
    private sealed class SubsetWalk
    {
        public int Index { get; set; }

        public int Size { get; set; }

        public HashMap<int, int> Frequency { get; } = new();
    }
}
