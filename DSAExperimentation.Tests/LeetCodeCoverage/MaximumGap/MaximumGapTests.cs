using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumGap;

// LeetCode 164. Maximum Gap: sort with this repo's MergeSort over
// ArrayIndexedSequence, then scan adjacent elements in the sorted order for
// the largest gap between successive values.
public sealed partial class MaximumGapTests
{
    [Theory]
    [InlineData(new[] { 3, 6, 9, 1 }, 3)]
    [InlineData(new[] { 10 }, 0)]
    [InlineData(new[] { 1, 1, 1, 1 }, 0)]
    public void MaximumGap_Examples_ReturnsLargestSortedNeighborGap(int[] nums, int expected) => Assert.Equal(expected, MaxGap(nums));

    private static int MaxGap(int[] nums)
    {
        if (nums.Length < 2)
        {
            return 0;
        }

        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var maxGap = 0;
        for (var i = 1; i < sorted.Length; i++)
        {
            maxGap = Math.Max(maxGap, sorted[i] - sorted[i - 1]);
        }

        return maxGap;
    }
}
