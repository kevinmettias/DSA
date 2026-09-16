using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedArrayII;

// Harness only. Both strategies are RemoveDuplicatesFromSortedArrayIISolution's -
// this file pins them to LeetCode's published examples, asserting both the
// reported length and the compacted at-most-twice prefix it describes.
public sealed partial class RemoveDuplicatesFromSortedArrayIITests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [0, 0, 1, 1, 1, 1, 2, 3, 3], 7, [0, 0, 1, 1, 2, 3, 3] },
            { [1, 1, 1, 2, 2, 3], 5, [1, 1, 2, 2, 3] },
            { [1], 1, [1] },
            { [], 0, [] },
            { [1, 2, 3], 3, [1, 2, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveDuplicatesByLinqGroupCapTwo_LeetCodeExamples_CompactsAtMostTwoPrefix(
        int[] nums, int expectedLength, int[] expectedPrefix)
    {
        var length = RemoveDuplicatesFromSortedArrayIISolution.RemoveDuplicatesByLinqGroupCapTwo(nums);

        Assert.Equal(expectedLength, length);
        Assert.Equal(expectedPrefix, nums[..length]);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveDuplicatesByArrayIndexedSequenceCompact_LeetCodeExamples_CompactsAtMostTwoPrefix(
        int[] nums, int expectedLength, int[] expectedPrefix)
    {
        var length = RemoveDuplicatesFromSortedArrayIISolution.RemoveDuplicatesByArrayIndexedSequenceCompact(nums);

        Assert.Equal(expectedLength, length);
        Assert.Equal(expectedPrefix, nums[..length]);
    }
}
