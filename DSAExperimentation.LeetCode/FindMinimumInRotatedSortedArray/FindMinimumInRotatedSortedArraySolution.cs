using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.FindMinimumInRotatedSortedArray;

// LeetCode 153. Find Minimum in Rotated Sorted Array: an ascending array with no
// duplicate values (LC 154 relaxes that), rotated at an unknown pivot. Every
// element before the pivot is > nums[^1]; every element from the pivot on is
// <= nums[^1] - so "is nums[i] <= nums[^1]" is a monotone rule over the indices,
// and this repo's own MonotonePredicateSearch.FirstTrue lands exactly on the
// pivot, which is the minimum. Same index-rule idiom PeakIndexInAMountainArray
// uses for its own unimodal search.
internal static class FindMinimumInRotatedSortedArraySolution
{
    // The textbook answer: walk the whole array. O(n), no repo primitive.
    public static int FindMinByLinearScan(int[] nums)
    {
        var min = nums[0];

        for (var i = 1; i < nums.Length; i++)
        {
            min = Math.Min(min, nums[i]);
        }

        return min;
    }

    // O(log n): MonotonePredicateSearch.FirstTrue over the indices. The last index
    // always holds, so the search always lands inside the array.
    public static int FindMinByPredicateSearch(int[] nums) =>
        nums[MonotonePredicateSearch.FirstTrue(0, nums.Length - 1, new OnLowSideOfRotation(nums))];

    // Holds(index) is true exactly when nums[index] is on the low side of the
    // rotation (at or below the last element) - the pivot itself is the first
    // such index, and the no-duplicates precondition is what keeps this rule
    // monotone, a law MonotonePredicateSearch assumes but never checks.
    private readonly struct OnLowSideOfRotation(int[] nums) : IMonotonePredicate<int>
    {
        public bool Holds(int index) => nums[index] <= nums[^1];
    }
}
