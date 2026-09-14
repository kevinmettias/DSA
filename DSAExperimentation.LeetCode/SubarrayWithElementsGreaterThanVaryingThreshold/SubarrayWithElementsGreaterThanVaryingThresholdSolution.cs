using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SubarrayWithElementsGreaterThanVaryingThreshold;

// LeetCode 2334. Subarray With Elements Greater Than Varying Threshold: find any
// subarray of size k whose every element exceeds threshold / k - equivalently, whose
// minimum times its length exceeds threshold - and report that size, or -1 when no
// subarray qualifies. LeetCode accepts *any* qualifying size, which is why the two
// strategies below are allowed to disagree on which one they report.
//
//   WindowMinimumScan is the textbook answer: for every start index extend the window
//   right while tracking a running minimum. O(n^2), all BCL.
//
//   UnionFindOrder sorts the indices by value descending (this repo's own
//   MergeSort.Sort<Element, TSequence> over an ArrayIndexedSequence<int> - the same
//   "sort an index array by a comparer closed over the value array" shape
//   OddEvenJumpSolution uses), then processes indices in that order, growing each
//   one's window with DisjointSet.Union over any already-processed neighbor
//   (GraphConnectivityWithThresholdSolution's own DisjointSet precedent, just unioning
//   adjacent indices instead of divisor multiples). Because values are processed
//   non-increasing, every neighbor already merged into an index's component holds a
//   value >= the one being placed, so that component's length is a valid window size
//   the moment length * value clears the threshold. O(n log n) for the sort plus
//   O(n * alpha(n)) for the union sweep.
//
// DisjointSet tracks the partition, not its component sizes, so a plain int[] indexed
// by (unioned) root carries the sizes alongside it - the same "DisjointSet plus
// caller-owned bookkeeping" shape a union-by-size policy would need if DisjointSet
// exposed one (ARCHITECTURE.md §10.1: linking policy is a complexity law, not
// something that type has to expose).
internal static class SubarrayWithElementsGreaterThanVaryingThresholdSolution
{
    // The partition of already-processed indices into maximal runs, plus each root's
    // run length and the processed flag that says which neighbors may be merged.
    private readonly record struct IndexWindows(DisjointSet Components, int[] Size, bool[] Visited);

    // The textbook arm, deliberately all-BCL: a quadratic forward scan carrying the
    // window's running minimum, returning the first window whose length * minimum
    // clears the threshold. It is the arm the composed strategy below has to justify
    // itself against.
    public static int ValidSubarraySizeByWindowMinimumScan(int[] nums, int threshold)
    {
        for (var start = 0; start < nums.Length; start++)
        {
            var minimum = int.MaxValue;

            for (var end = start; end < nums.Length; end++)
            {
                minimum = Math.Min(minimum, nums[end]);
                var length = end - start + 1;

                if ((long)length * minimum > threshold)
                {
                    return length;
                }
            }
        }

        return LeetCodeAnswer.None;
    }

    // The composed arm: MergeSort over the index order plus a DisjointSet sweep.
    public static int ValidSubarraySizeByUnionFindOrder(int[] nums, int threshold)
    {
        var windows = new IndexWindows(
            new DisjointSet(nums.Length), new int[nums.Length], new bool[nums.Length]);

        foreach (var index in DescendingValueOrder(nums))
        {
            var size = AbsorbProcessedNeighbors(windows, index);

            if ((long)size * nums[index] > threshold)
            {
                return size;
            }
        }

        return LeetCodeAnswer.None;
    }

    private static int[] DescendingValueOrder(int[] nums)
    {
        var order = Enumerable.Range(0, nums.Length).ToArray();
        var byDescendingValueThenIndex = Comparer<int>.Create((a, b) => CompareByValueDescending(nums, a, b));

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(
            new ArrayIndexedSequence<int>(order), byDescendingValueThenIndex);

        return order;
    }

    // Ties are broken toward the smaller index so the order is total, which keeps the
    // sweep below deterministic rather than dependent on the sort's own tie handling.
    private static int CompareByValueDescending(int[] nums, int first, int second)
    {
        var sameValue = nums[first] == nums[second];

        return sameValue ? first.CompareTo(second) : nums[second].CompareTo(nums[first]);
    }

    // Marks index as processed and merges it with whichever immediate neighbors already
    // are, returning the length of the run it now belongs to. Every merged neighbor was
    // processed earlier and therefore holds a value >= nums[index], so nums[index] is
    // the run's minimum.
    private static int AbsorbProcessedNeighbors(IndexWindows windows, int index)
    {
        windows.Visited[index] = true;
        windows.Size[index] = 1;

        if (index > 0 && windows.Visited[index - 1])
        {
            MergeInto(windows, index, index - 1);
        }

        if (index < windows.Visited.Length - 1 && windows.Visited[index + 1])
        {
            MergeInto(windows, index, index + 1);
        }

        return windows.Size[windows.Components.Find(index)];
    }

    private static void MergeInto(IndexWindows windows, int index, int neighbor)
    {
        var combined = windows.Size[windows.Components.Find(index)]
            + windows.Size[windows.Components.Find(neighbor)];

        windows.Components.Union(index, neighbor);
        windows.Size[windows.Components.Find(index)] = combined;
    }
}
