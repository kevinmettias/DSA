using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SearchInRotatedSortedArray;

// LeetCode 33. Search in Rotated Sorted Array: find target's index in an
// ascending array that has been rotated at some unknown pivot, or -1.
//
// SearchByBinarySearchPivotAndSlice locates the pivot with one
// MonotonePredicateSearch.FirstTrue pass over a predicate that is monotone across
// the rotation ("does this slot belong to the tail of the original sorted run?"),
// then runs a BinarySearch.Find confined to whichever half - [0, pivot) or
// [pivot, length) - could actually contain target, decided by comparing target
// against the array's own last element. SearchByLinearScan is the O(n) arm it has
// to beat.
internal static class SearchInRotatedSortedArraySolution
{
    // The textbook O(n) scan. Written without this repo's own BinarySearch -
    // the arm BinarySearchPivotAndSlice has to beat.
    public static int SearchByLinearScan(int[] nums, int target) => Array.IndexOf(nums, target);

    public static int SearchByBinarySearchPivotAndSlice(int[] nums, int target)
    {
        if (nums.Length == 0)
        {
            return -1;
        }

        var pivot = MonotonePredicateSearch.FirstTrue(0, nums.Length - 1, new InTailRun(nums));
        var searchRight = target <= nums[^1];
        var start = searchRight ? pivot : 0;
        var length = searchRight ? RightWindowLength(nums, pivot) : pivot;

        // The chosen half is a plain sorted window [start, start + length) of nums,
        // reindexed from 0 so BinarySearch.Find can treat it like any other sorted
        // sequence.
        var found = BinarySearch.Find<int, OffsetSequence<int>>(new OffsetSequence<int>(nums, start, length), target);

        return found is null ? -1 : AbsoluteIndex(start, found.Value);
    }

    private static int RightWindowLength(int[] nums, int pivot) => nums.Length - pivot;

    private static int AbsoluteIndex(int start, int offset) => start + offset;

    // Holds(index) is true exactly when nums[index] belongs to the original sorted
    // run's tail - at or below the last element: false before the pivot and true
    // from it on, so FirstTrue lands exactly on the pivot. The last index always
    // holds, so the pivot always lies inside the array.
    private readonly struct InTailRun(int[] nums) : IMonotonePredicate<int>
    {
        public bool Holds(int index) => nums[index] <= nums[^1];
    }
}
