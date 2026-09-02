using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.TwoSumIIInputArrayIsSorted;

// LeetCode 167. Two Sum II - Input Array Is Sorted: return the 1-indexed
// positions of the two entries that sum to target, given the array is
// already sorted ascending.
//
// For each first index i, BinarySearch.Find over the sorted suffix that
// follows it locates the complement in O(log n) - reusing this repo's own
// OffsetSequence<int> to hand BinarySearch that suffix without copying it
// into a fresh array first.
internal static class TwoSumIIInputArrayIsSortedSolution
{
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
