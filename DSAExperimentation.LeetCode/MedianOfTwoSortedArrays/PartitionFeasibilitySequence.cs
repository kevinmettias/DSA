using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MedianOfTwoSortedArrays;

// IRandomAccessSequence<int> witness over partition index i in [0, nums1.Length]:
// Get(i) is 0 while nums1[i-1] <= nums2[half-i] (this partition still leaves every
// element to nums1's left no bigger than nums2's right-hand neighbor) and flips to
// 1 once it doesn't - a single monotonic step, never re-examined once it flips, so
// BinarySearch.LowerBound's existing loop finds the boundary in O(log m) without
// ever touching nums2's own length. This answers LC 4's partition search and
// nothing else, which is why it lives beside the solution rather than in
// DataStructures or Domain.
internal readonly struct PartitionFeasibilitySequence(int[] nums1, int[] nums2, int half) : IRandomAccessSequence<int>
{
    public int Length => nums1.Length + 1;

    public int Get(int i)
    {
        var j = half - i;
        var leftOfNums1 = i == 0 ? int.MinValue : ValueAt(nums1, i - 1);
        var rightOfNums2 = j == nums2.Length ? int.MaxValue : ValueAt(nums2, j);
        return leftOfNums1 <= rightOfNums2 ? 0 : 1;
    }

    private static int ValueAt(int[] nums, int index) => nums[index];
}
