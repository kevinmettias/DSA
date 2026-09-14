using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.MaximumSegmentSumAfterRemovals;

// LeetCode 2382. Maximum Segment Sum After Removals: answer[i] is the largest sum
// of any remaining contiguous segment after nums[removeQueries[0..i]] have all
// been removed.
//
// Removal only ever splits segments, which no incremental structure handles well -
// so the composed strategy runs time backwards. Replayed in reverse, every removal
// becomes an INSERTION into an initially-all-removed array, and insertion only
// merges segments, which is exactly what a disjoint set does. A per-root running
// sum threaded beside Find/Union (BricksFallingWhenHit's own reverse-time trick,
// with a sum where that problem keeps a size) gives each restored segment's total,
// and because a merge can only grow a segment the best sum seen so far never
// shrinks - so one running maximum, recorded as the pass unwinds, reconstructs the
// whole answer array in forward order.
internal static class MaximumSegmentSumAfterRemovalsSolution
{
    // The textbook answer: mark the queried index removed, then rescan the whole
    // array for the best surviving run, once per query - O(n^2). Deliberately
    // written without this repo's DisjointSet, using nothing but a bool[] and two
    // running longs; it is the arm the composed strategy below has to justify
    // itself against.
    public static long[] MaximumSegmentSumsByRescanAfterEachRemoval(int[] nums, int[] removeQueries)
    {
        var answer = new long[nums.Length];
        var removed = new bool[nums.Length];

        for (var i = 0; i < removeQueries.Length; i++)
        {
            removed[removeQueries[i]] = true;
            answer[i] = LargestRemainingSegment(nums, removed);
        }

        return answer;
    }

    private static long LargestRemainingSegment(int[] nums, bool[] removed)
    {
        var best = 0L;
        var current = 0L;

        for (var i = 0; i < nums.Length; i++)
        {
            current = removed[i] ? 0L : current + nums[i];
            best = Math.Max(best, current);
        }

        return best;
    }

    // This repo's own DisjointSet, walked backwards: answer[n-1] is 0 because the
    // last query removes the final index, and each earlier answer is the running
    // maximum segment sum observed as the removals are undone one at a time.
    public static long[] MaximumSegmentSumsByReverseTimeDisjointSet(int[] nums, int[] removeQueries)
    {
        var n = nums.Length;
        var answer = new long[n];
        var segments = new SegmentSums(new DisjointSet(n), new bool[n], new long[n], nums);
        var maxSum = 0L;

        for (var i = n - 1; i >= 1; i--)
        {
            var index = removeQueries[i];
            Restore(segments, index);
            maxSum = Math.Max(maxSum, segments.Sum[segments.Components.Find(index)]);
            answer[i - 1] = maxSum;
        }

        return answer;
    }

    // Bring one index back and absorb whichever neighbouring segments are already
    // standing, so the sum on the merged root stays the segment's true total.
    private static void Restore(SegmentSums segments, int index)
    {
        segments.Present[index] = true;
        segments.Sum[index] = segments.Nums[index];

        if (index > 0 && segments.Present[index - 1])
        {
            Merge(segments, index, index - 1);
        }

        if (index < segments.Nums.Length - 1 && segments.Present[index + 1])
        {
            Merge(segments, index, index + 1);
        }
    }

    private static void Merge(SegmentSums segments, int first, int second)
    {
        var firstRoot = segments.Components.Find(first);
        var secondRoot = segments.Components.Find(second);

        segments.Components.Union(first, second);
        segments.Sum[segments.Components.Find(first)] = segments.Sum[firstRoot] + segments.Sum[secondRoot];
    }

    // The scratch state the reverse-time pass threads through Find/Union: which
    // indices have been restored, and the running total on each segment's root.
    // Bundled the same way OpenTheLockSolution's TurnWalk bundles its BFS state.
    private readonly record struct SegmentSums(
        DisjointSet Components,
        bool[] Present,
        long[] Sum,
        int[] Nums);
}
