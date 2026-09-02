using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArrayII;

// LeetCode 80. Remove Duplicates from Sorted Array II: compact a sorted array in
// place so no value occurs more than twice, returning the new length - the same
// read/write two-pointer shape RemoveDuplicatesFromSortedArraySolution (LC 26)
// uses, generalized from "keep if different from the last kept element" to "keep
// if different from the element kept two slots back", which is exactly what caps
// every run at 2 occurrences instead of 1.
//
// The two strategies differ only in what walks the compaction - LINQ's own
// GroupBy capped per group, or a read/write cursor pair over this repo's
// ArrayIndexedSequence.
internal static class RemoveDuplicatesFromSortedArrayIISolution
{
    private const int MaxAllowedDuplicates = 2;

    // The textbook answer: GroupBy keeps first-seen order, which on an
    // already-sorted array groups each run of equal values together in order;
    // taking at most 2 from each group and writing them back over nums is exactly
    // LC 80's "at most twice" rule. Deliberately written without this repo's
    // sequence primitive.
    public static int RemoveDuplicatesByLinqGroupCapTwo(int[] nums)
    {
        var kept = nums.GroupBy(x => x).SelectMany(g => g.Take(MaxAllowedDuplicates)).ToArray();
        Array.Copy(kept, nums, kept.Length);

        return kept.Length;
    }

    // The composed solution: advance a read cursor over every element, only
    // advancing the write cursor (and copying the value back) when the read
    // cursor finds a value that would not push its own run past 2 occurrences
    // among what the write cursor has already placed.
    public static int RemoveDuplicatesByArrayIndexedSequenceCompact(int[] nums)
    {
        var sequence = new ArrayIndexedSequence<int>(nums);
        var write = 0;

        for (var read = 0; read < sequence.Length; read++)
        {
            if (write < MaxAllowedDuplicates || sequence.Get(read) != sequence.Get(write - MaxAllowedDuplicates))
            {
                sequence.Set(write++, sequence.Get(read));
            }
        }

        return write;
    }
}
