using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestUnsortedContinuousSubarray;

// LeetCode 581. Shortest Unsorted Continuous Subarray: sort a copy with this
// repo's own MergeSort over ArrayIndexedSequence - the same MaximumGapTests
// shape - then the shortest subarray that needs re-sorting is exactly the span
// between the leftmost and rightmost indices where the original and sorted
// arrays disagree.
public sealed partial class ShortestUnsortedContinuousSubarrayTests
{
    [Theory]
    [InlineData(new[] { 2, 6, 4, 8, 10, 9, 15 }, 5)]
    [InlineData(new[] { 1, 2, 3, 4 }, 0)]
    [InlineData(new[] { 1 }, 0)]
    public void FindUnsortedSubarray_Examples_ReturnsShortestSpanLength(int[] nums, int expected)
        => Assert.Equal(expected, FindUnsortedSubarray(nums));

    private static int FindUnsortedSubarray(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var left = 0;
        while (left < nums.Length && nums[left] == sorted[left])
        {
            left++;
        }

        if (left == nums.Length)
        {
            return 0;
        }

        var right = nums.Length - 1;
        while (nums[right] == sorted[right])
        {
            right--;
        }

        return right - left + 1;
    }
}
