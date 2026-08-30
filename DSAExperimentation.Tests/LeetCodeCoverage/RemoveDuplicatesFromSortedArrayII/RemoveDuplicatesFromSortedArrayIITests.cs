using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedArrayII;

// LeetCode 80. Remove Duplicates from Sorted Array II: the same read/write two-
// pointer compaction over this repo's own ArrayIndexedSequence<int> that
// RemoveDuplicatesFromSortedArrayTests (LC 26) already uses, generalized from
// "keep if different from the last kept element" to "keep if different from the
// element kept two slots back" - which is exactly what caps every run at 2
// occurrences instead of 1.
public sealed partial class RemoveDuplicatesFromSortedArrayIITests
{
    [Theory]
    [InlineData(new[] { 0, 0, 1, 1, 1, 1, 2, 3, 3 }, 7, new[] { 0, 0, 1, 1, 2, 3, 3 })]
    [InlineData(new[] { 1, 1, 1, 2, 2, 3 }, 5, new[] { 1, 1, 2, 2, 3 })]
    [InlineData(new[] { 1 }, 1, new[] { 1 })]
    public void RemoveDuplicates_ArrayIndexedSequenceCompaction_AllowsAtMostTwoOccurrences(
        int[] nums, int expectedLength, int[] expectedPrefix)
    {
        var length = RemoveDuplicates(nums);

        Assert.Equal(expectedLength, length);
        Assert.Equal(expectedPrefix, nums[..length]);
    }

    private static int RemoveDuplicates(int[] nums)
    {
        var sequence = new ArrayIndexedSequence<int>(nums);
        var write = 0;

        for (var read = 0; read < sequence.Length; read++)
        {
            if (write < 2 || sequence.Get(read) != sequence.Get(write - 2))
            {
                sequence.Set(write++, sequence.Get(read));
            }
        }

        return write;
    }
}
