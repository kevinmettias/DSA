using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.SumOfAllSubsetXORTotals;

// LeetCode 1863. Sum of All Subset XOR Totals: add up the XOR total of every one of
// the 2^n subsets of nums, the empty subset (total 0) included.
//
// Both strategies enumerate all 2^n subsets; they differ in what each subset costs.
// The bitmask baseline recomputes a subset's XOR from scratch by rescanning every
// element - O(2^n * n). The backtracking arm walks this repo's own Backtrack.Search
// choose/explore/unchoose recursion, where every node of the recursion tree IS a
// subset, so IsSolution is unconditionally true and one running XOR is toggled by a
// single XOR on Choose and untoggled by the same XOR on Unchoose - O(2^n).
internal static class SumOfAllSubsetXORTotalsSolution
{
    // The textbook answer: iterate the 2^n bitmasks and rescan the array for each
    // one. Deliberately BCL-only - it is the arm the backtracking walk below has to
    // justify itself against.
    public static int SubsetXorSumByBitmask(int[] nums)
    {
        var total = 0;
        var subsetCount = 1 << nums.Length;

        for (var mask = 0; mask < subsetCount; mask++)
        {
            total += XorOfSelected(nums, mask);
        }

        return total;
    }

    private static int XorOfSelected(int[] nums, int mask)
    {
        var xorTotal = 0;

        for (var bit = 0; bit < nums.Length; bit++)
        {
            if ((mask & (1 << bit)) != 0)
            {
                xorTotal ^= nums[bit];
            }
        }

        return xorTotal;
    }

    // The same enumeration as one incremental walk: Choose toggles nums[index] into
    // the running XOR, Unchoose toggles the identical value back out, and every node
    // visited contributes its current running XOR - the exhaustive-enumeration shape
    // Subsets already uses for LC 78, accumulating instead of collecting snapshots.
    public static int SubsetXorSumByBacktracking(int[] nums)
    {
        var total = 0;
        var state = new XorState();

        Backtrack.Search<XorState, int>(
            state,
            isSolution: static _ => true,
            candidates: s => Enumerable.Range(s.NextIndex, nums.Length - s.NextIndex),
            choose: (s, index) =>
            {
                s.RunningXor ^= nums[index];
                s.NextIndex = index + 1;
            },
            unchoose: (s, index) => s.RunningXor ^= nums[index],
            onSolution: s => total += s.RunningXor);

        return total;
    }

    // The mutable state the choose/explore/unchoose walk threads: the XOR of the
    // chosen elements, and the first index still free to choose.
    private sealed class XorState
    {
        public int RunningXor { get; set; }

        public int NextIndex { get; set; }
    }
}
