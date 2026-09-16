using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedArray;

// Harness only. Both strategies are RemoveDuplicatesFromSortedArraySolution's -
// this file pins them to LeetCode's published examples, asserting both the
// reported length and the compacted unique prefix it describes.
public sealed partial class RemoveDuplicatesFromSortedArrayTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 1, 2], 2, [1, 2] },
            { [0, 0, 1, 1, 1, 2, 2, 3, 3, 4], 5, [0, 1, 2, 3, 4] },
            { [1], 1, [1] },
            { [1, 2, 3], 3, [1, 2, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveDuplicatesByLinqDistinct_LeetCodeExamples_CompactsUniquePrefix(
        int[] nums, int expectedLength, int[] expectedPrefix)
    {
        var length = RemoveDuplicatesFromSortedArraySolution.RemoveDuplicatesByLinqDistinct(nums);

        Assert.Equal(expectedLength, length);
        Assert.Equal(expectedPrefix, nums[..length]);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveDuplicatesByArrayIndexedSequenceCompact_LeetCodeExamples_CompactsUniquePrefix(
        int[] nums, int expectedLength, int[] expectedPrefix)
    {
        var length = RemoveDuplicatesFromSortedArraySolution.RemoveDuplicatesByArrayIndexedSequenceCompact(nums);

        Assert.Equal(expectedLength, length);
        Assert.Equal(expectedPrefix, nums[..length]);
    }
}
