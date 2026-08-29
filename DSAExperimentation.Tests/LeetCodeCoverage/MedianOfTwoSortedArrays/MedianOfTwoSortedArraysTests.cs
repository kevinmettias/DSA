using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MedianOfTwoSortedArrays;

// LeetCode 4. Median of Two Sorted Arrays: binary-searches the smaller array for the
// partition index using this repo's own generic BinarySearch.LowerBound - the same
// bisection loop BinarySearch.cs already uses to search a concrete sorted array,
// reused unmodified here to bisect on a synthetic "is this partition still feasible"
// predicate instead. That predicate (PartitionFeasibilitySequence below) is monotonic
// (0 while feasible, 1 once it stops being feasible), i.e. already "sorted ascending"
// in exactly the shape LowerBound assumes, so no change to BinarySearch itself is
// needed to generalize it from array search to search-on-answer.
public sealed partial class MedianOfTwoSortedArraysTests
{
    [Fact]
    public void FindMedianSortedArrays_OddTotalLength_ReturnsMiddleElement()
        => Assert.Equal(2, FindMedianSortedArrays([1, 3], [2]));

    [Fact]
    public void FindMedianSortedArrays_EvenTotalLength_ReturnsAverageOfMiddleTwo()
        => Assert.Equal(2.5, FindMedianSortedArrays([1, 2], [3, 4]));

    [Fact]
    public void FindMedianSortedArrays_FirstArrayEmpty_ReturnsOtherArraysMedian()
        => Assert.Equal(2, FindMedianSortedArrays([], [1, 2, 3]));

    [Fact]
    public void FindMedianSortedArrays_NoOverlapInValueRanges_ReturnsCorrectMedian()
        => Assert.Equal(3.5, FindMedianSortedArrays([1, 2, 3], [4, 5, 6]));

    [Fact]
    public void FindMedianSortedArrays_SingleElementEach_ReturnsAverage()
        => Assert.Equal(1.5, FindMedianSortedArrays([1], [2]));

    private static double FindMedianSortedArrays(int[] nums1, int[] nums2)
    {
        if (nums1.Length > nums2.Length)
        {
            return FindMedianSortedArrays(nums2, nums1);
        }

        var m = nums1.Length;
        var n = nums2.Length;
        var half = (m + n + 1) / 2;

        var feasibility = new PartitionFeasibilitySequence(nums1, nums2, half);
        var i = BinarySearch.LowerBound<int, PartitionFeasibilitySequence>(feasibility, 1) - 1;
        var j = half - i;

        var leftOfPartition = Math.Max(
            i == 0 ? int.MinValue : nums1[i - 1],
            j == 0 ? int.MinValue : nums2[j - 1]);

        if ((m + n) % 2 == 1)
        {
            return leftOfPartition;
        }

        var rightOfPartition = Math.Min(
            i == m ? int.MaxValue : nums1[i],
            j == n ? int.MaxValue : nums2[j]);

        return (leftOfPartition + rightOfPartition) / 2.0;
    }

    // IRandomAccessSequence<int> witness over partition index i in [0, nums1.Length]:
    // Get(i) is 0 while nums1[i-1] <= nums2[half-i] (this partition still leaves every
    // element to nums1's left no bigger than nums2's right-hand neighbor) and flips to
    // 1 once it doesn't - a single monotonic step, never re-examined once it flips, so
    // LowerBound's existing loop finds the boundary in O(log m) without ever touching
    // nums2's own length.
    private readonly struct PartitionFeasibilitySequence(int[] nums1, int[] nums2, int half) : IRandomAccessSequence<int>
    {
        public int Length => nums1.Length + 1;

        public int Get(int i)
        {
            var j = half - i;
            var leftOfNums1 = i == 0 ? int.MinValue : nums1[i - 1];
            var rightOfNums2 = j == nums2.Length ? int.MaxValue : nums2[j];
            return leftOfNums1 <= rightOfNums2 ? 0 : 1;
        }
    }
}
