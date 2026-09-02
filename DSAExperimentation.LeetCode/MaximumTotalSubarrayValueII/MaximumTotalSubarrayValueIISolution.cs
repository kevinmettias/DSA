using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MaximumTotalSubarrayValueII;

// LeetCode 3691. Maximum Total Subarray Value II: pick exactly k distinct
// subarrays nums[l..r] (overlaps allowed, no repeats) to maximize the sum of each
// one's value, max(l..r) - min(l..r).
//
// For a fixed left endpoint l, value(l, r) is monotonically non-decreasing as r
// grows: extending the window can only raise its max and can only lower its min,
// since the window only grows. So for fixed l, value(l, n-1) >= value(l, n-2) >=
// ... >= value(l, l) - exactly LC 373 "K Pairs with Smallest Sums"'s row-sorted
// shape. Seed a max-heap with (value(l, n-1), l, n-1) for every l, and whenever
// (l, r) is popped, only THEN reveal (l, r-1) - never before, since the row's
// monotonicity is only known in that one direction. Popping k times greedily
// recovers the true top-k SET this way, the same lazy-heap correctness argument
// LC 373 relies on. Range max/min per candidate is exactly
// DataStructures.SegmentTree's role, and DataStructures.Heap.Heap already exists
// for the priority-queue side.
internal static class MaximumTotalSubarrayValueIISolution
{
    // What you would write without this repo: recompute every subarray's value
    // directly - a running max/min as r grows for each fixed l - collect them all,
    // and sum the k largest.
    public static long MaxTotalValueByBruteForce(int[] nums, int k)
    {
        var values = new List<long>();

        for (var l = 0; l < nums.Length; l++)
        {
            var max = nums[l];
            var min = nums[l];

            for (var r = l; r < nums.Length; r++)
            {
                max = Math.Max(max, nums[r]);
                min = Math.Min(min, nums[r]);
                values.Add(max - min);
            }
        }

        values.Sort((left, right) => right.CompareTo(left));

        var total = 0L;

        for (var i = 0; i < k; i++)
        {
            total += values[i];
        }

        return total;
    }

    // Composed: two SegmentTrees answer max(l..r)/min(l..r) in O(log n), and a
    // Heap<SubarrayCandidate, MaxHeapOrder<SubarrayCandidate>> runs the lazy
    // top-k walk described above.
    public static long MaxTotalValueBySegmentTreeHeap(int[] nums, int k) =>
        MaxTotalValueBySegmentTreeHeap(
            new SegmentTree<int, MaxOperation<int>>(nums),
            new SegmentTree<int, MinOperation<int>>(nums),
            k);

    public static long MaxTotalValueBySegmentTreeHeap(
        SegmentTree<int, MaxOperation<int>> maxTree, SegmentTree<int, MinOperation<int>> minTree, int k)
    {
        var heap = new Heap<SubarrayCandidate, MaxHeapOrder<SubarrayCandidate>>();

        for (var left = 0; left < maxTree.Count; left++)
        {
            heap.Push(MakeCandidate(maxTree, minTree, left, maxTree.Count - 1));
        }

        var total = 0L;

        for (var taken = 0; taken < k; taken++)
        {
            heap.TryPop(out var candidate);
            total += candidate.Value;

            if (candidate.Right > candidate.Left)
            {
                heap.Push(MakeCandidate(maxTree, minTree, candidate.Left, candidate.Right - 1));
            }
        }

        return total;
    }

    private static SubarrayCandidate MakeCandidate(
        SegmentTree<int, MaxOperation<int>> maxTree, SegmentTree<int, MinOperation<int>> minTree, int left, int right) =>
        new((long)maxTree.Query(left, right) - minTree.Query(left, right), left, right);

    // Meaningless outside this problem's lazy top-k walk, so it stays in this
    // problem's own folder rather than a shared tier (ARCHITECTURE.md 17.3).
    private readonly record struct SubarrayCandidate(long Value, int Left, int Right) : IComparable<SubarrayCandidate>
    {
        public int CompareTo(SubarrayCandidate other) => Value.CompareTo(other.Value);
    }
}
