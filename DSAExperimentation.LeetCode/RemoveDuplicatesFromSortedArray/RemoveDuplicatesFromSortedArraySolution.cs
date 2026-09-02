using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArray;

// LeetCode 26. Remove Duplicates from Sorted Array: compact a sorted array's
// unique values into its own prefix in place, returning the new length.
//
// The two strategies differ only in what walks the compaction - LINQ's own
// Distinct, or a read/write cursor pair over this repo's ArrayIndexedSequence.
internal static class RemoveDuplicatesFromSortedArraySolution
{
    // The textbook answer: LINQ's Distinct() keeps first-seen order, which on an
    // already-sorted array is exactly "the unique values, in order" - write that
    // sequence back over nums and report its length. Deliberately written without
    // this repo's sequence primitive.
    public static int RemoveDuplicatesByLinqDistinct(int[] nums)
    {
        var distinct = nums.Distinct().ToArray();
        Array.Copy(distinct, nums, distinct.Length);

        return distinct.Length;
    }

    // The composed solution: advance a read cursor over every element, only
    // advancing the write cursor (and copying the value back) when the read
    // cursor finds something the write cursor hasn't already placed.
    public static int RemoveDuplicatesByArrayIndexedSequenceCompact(int[] nums)
    {
        if (nums.Length == 0)
        {
            return 0;
        }

        var sequence = new ArrayIndexedSequence<int>(nums);
        var write = 1;

        for (var read = 1; read < sequence.Length; read++)
        {
            if (sequence.Get(read) != sequence.Get(write - 1))
            {
                sequence.Set(write++, sequence.Get(read));
            }
        }

        return write;
    }
}
