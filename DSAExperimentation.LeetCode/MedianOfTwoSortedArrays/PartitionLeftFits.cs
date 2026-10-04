using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.MedianOfTwoSortedArrays;

// The partition rule over nums1's partition index in [0, nums1.Length]: Holds(index) is
// true while nums1[index-1] <= nums2[half-index] (this partition still leaves every
// element to nums1's left no bigger than nums2's right-hand neighbor) and turns false
// once it doesn't - a single monotone step, so MonotonePredicateSearch.LastTrue finds
// the last partition that fits in O(log m) without ever touching nums2's own length.
// Index 0 always fits, its left side being empty. This answers LC 4's partition search
// and nothing else, which is why it lives beside the solution rather than in
// DataStructures or Domain.
internal readonly struct PartitionLeftFits(int[] nums1, int[] nums2, int half) : IMonotonePredicate<int>
{
    public bool Holds(int index)
    {
        var j = half - index;
        var leftOfNums1 = index == 0 ? int.MinValue : ValueAt(nums1, index - 1);
        var rightOfNums2 = j == nums2.Length ? int.MaxValue : ValueAt(nums2, j);
        return leftOfNums1 <= rightOfNums2;
    }

    private static int ValueAt(int[] nums, int index) => nums[index];
}
