using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SearchInRotatedSortedArray;

// LeetCode 33. Search in Rotated Sorted Array: find target's index in an
// ascending array that has been rotated at some unknown pivot, or -1.
//
// SearchByBinarySearchPivotAndSlice locates the pivot with one LowerBound pass
// over a predicate that is monotone across the rotation ("does this slot
// belong to the tail of the original sorted run?"), then runs a second search
// confined to whichever half - [0, pivot) or [pivot, length) - could actually
// contain target, decided by comparing target against the array's own last
// element. SearchByLinearScan is the O(n) arm it has to beat.
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

        var pivot = BinarySearch.LowerBound<int, PivotSequence>(new PivotSequence(nums), 1);
        var searchRight = target <= nums[^1];
        var start = searchRight ? pivot : 0;
        var length = searchRight ? nums.Length - pivot : pivot;
        var found = BinarySearch.Find<int, OffsetSequence>(new OffsetSequence(nums, start, length), target);

        return found is null ? -1 : start + found.Value;
    }

    // 1 past the pivot, 0 up to and including it: LowerBound on this predicate
    // lands exactly on the first index belonging to the array's tail run.
    private readonly struct PivotSequence(int[] nums) : IRandomAccessSequence<int>
    {
        public int Length => nums.Length;
        public int Get(int index) => nums[index] <= nums[^1] ? 1 : 0;
    }

    // A plain sorted window [start, start + length) of nums, reindexed from 0
    // so BinarySearch.Find can treat it like any other sorted sequence.
    private readonly struct OffsetSequence(int[] nums, int start, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int index) => nums[start + index];
    }
}
