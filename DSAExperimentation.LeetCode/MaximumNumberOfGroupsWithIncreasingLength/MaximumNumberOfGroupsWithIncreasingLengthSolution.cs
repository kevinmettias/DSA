using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumNumberOfGroupsWithIncreasingLength;

// LeetCode 2790. Maximum Number of Groups With Increasing Length: each index i may be
// used at most usageLimits[i] times, every group holds distinct indices, and each
// group must be strictly longer than the one before it - how many groups can be
// formed?
//
// The whole answer is "sort the limits ascending, then one linear greedy sweep": walk
// the sorted limits accumulating a running pool of available uses, and whenever the
// pool reaches the next group's required size, claim that group and spend the size.
// This is the standard exchange argument - processing small limits first spends a
// low-capacity index on an early, small group while high-capacity indices carry over
// to sustain the later, larger ones.
//
// Since the sweep is shared and linear, the only thing the two strategies differ in
// is the sort that precedes it.
internal static class MaximumNumberOfGroupsWithIncreasingLengthSolution
{
    // The textbook answer: an in-place O(n^2) insertion sort of a copy, then the same
    // sweep. Plain BCL arrays and nothing from this repo - it is the arm the merge
    // sort below has to justify itself against, and putting it here is what finally
    // gets it asserted.
    public static int MaxIncreasingGroupsByInsertionSort(int[] usageLimits)
    {
        var sorted = (int[])usageLimits.Clone();

        for (var i = 1; i < sorted.Length; i++)
        {
            InsertOne(sorted, i);
        }

        return GreedySweep(sorted);
    }

    // Shifts sorted[index] left past every larger value already placed before it.
    private static void InsertOne(int[] values, int index)
    {
        var value = values[index];
        var j = index - 1;

        while (j >= 0 && values[j] > value)
        {
            values[j + 1] = values[j];
            j--;
        }

        values[j + 1] = value;
    }

    // This repo's own MergeSort over an ArrayIndexedSequence<int> view of the copy,
    // O(n log n) instead of O(n^2), followed by the identical sweep.
    public static int MaxIncreasingGroupsByMergeSort(int[] usageLimits)
    {
        var sorted = (int[])usageLimits.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        return GreedySweep(sorted);
    }

    // The greedy itself, shared by both strategies so the only thing they differ in
    // is how the limits got sorted. The pool is a long because n limits of up to 1e9
    // each overflow an int long before the group sizes catch up.
    private static int GreedySweep(int[] sorted)
    {
        long available = 0;
        var groups = 0;
        var neededSize = 1;

        foreach (var limit in sorted)
        {
            available += limit;

            if (available >= neededSize)
            {
                groups++;
                available -= neededSize;
                neededSize++;
            }
        }

        return groups;
    }
}
