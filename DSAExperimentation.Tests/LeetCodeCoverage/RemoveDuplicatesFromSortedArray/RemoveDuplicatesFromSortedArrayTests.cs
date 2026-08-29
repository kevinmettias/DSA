using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedArray;

public sealed partial class RemoveDuplicatesFromSortedArrayTests
{
    [Theory]
    [InlineData(new[] { 1, 1, 2 }, 2, new[] { 1, 2 })]
    [InlineData(new[] { 0, 0, 1, 1, 1, 2, 2, 3, 3, 4 }, 5, new[] { 0, 1, 2, 3, 4 })]
    public void RemoveDuplicates_LeetCodeExamples_CompactsUniquePrefix(int[] nums, int expectedLength, int[] expectedPrefix)
    {
        var length = RemoveDuplicates(nums);

        Assert.Equal(expectedLength, length);
        Assert.Equal(expectedPrefix, nums[..length]);
    }

    private static int RemoveDuplicates(int[] nums)
    {
        if (nums.Length == 0) return 0;
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
