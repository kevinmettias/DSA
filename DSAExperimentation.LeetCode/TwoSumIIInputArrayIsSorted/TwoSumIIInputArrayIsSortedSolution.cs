using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.TwoSumIIInputArrayIsSorted;

// LeetCode 167. Two Sum II - Input Array Is Sorted: return the 1-indexed
// positions of the two entries that sum to target, given the array is
// already sorted ascending.
//
// Two strategies exploit the sort differently: a per-index binary search for the
// complement, or a single two-pointer squeeze that moves the low or high pointer
// according to whether the running sum is under or over target.
internal static class TwoSumIIInputArrayIsSortedSolution
{
    // Squeeze from both ends: while the sum is too small advance the low pointer,
    // while it is too large retreat the high one, and stop when they meet the
    // target. One linear pass against the binary-search arm's O(n log n), with no
    // per-index suffix view - at the cost of only being able to walk toward the
    // pair rather than seek any single complement directly.
    public static int[] TryFindIndicesByTwoPointerSqueeze(int[] nums, int target)
    {
        var left = 0;
        var right = nums.Length - 1;

        while (left < right)
        {
            var sum = nums[left] + nums[right];

            if (sum == target)
            {
                return [left + 1, right + 1];
            }

            if (sum < target)
            {
                left++;
            }
            else
            {
                right--;
            }
        }

        return [];
    }

    // The binary-search arm: for each first index i, BinarySearch.Find over the
    // sorted suffix that follows it locates the complement in O(log n) - reusing
    // this repo's own OffsetSequence<int> to hand BinarySearch that suffix without
    // copying it into a fresh array first.
    public static int[] TryFindIndicesByBinarySearch(int[] nums, int target)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            var suffix = new OffsetSequence<int>(nums, i + 1, nums.Length - i - 1);
            var found = BinarySearch.Find<int, OffsetSequence<int>>(suffix, target - nums[i]);

            if (found is not null)
            {
                return [i + 1, i + found.Value + 2];
            }
        }

        return [];
    }
}
