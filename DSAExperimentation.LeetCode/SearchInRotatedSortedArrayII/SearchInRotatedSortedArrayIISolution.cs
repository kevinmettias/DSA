using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SearchInRotatedSortedArrayII;

// LeetCode 81. Search in Rotated Sorted Array II: report whether target is
// present in an ascending array that has been rotated at some unknown pivot and
// may contain duplicates.
//
// Duplicates can make the rotation boundary ambiguous (nums[left] == nums[right]
// doesn't say which side the pivot is on), so HasTargetByTrimDuplicatesThenBinarySearch
// first trims matching values off the left edge - correct for a presence-only
// search because trimming nums[left] only ever happens while an equal-valued
// witness still sits at nums[right], so no value's last remaining occurrence is
// ever trimmed away. Once nums[left] != nums[right] (or the range collapses to one
// element), the remaining slice has no duplicate-boundary ambiguity left, so it is
// handed to this repo's own MonotonePredicateSearch.FirstTrue (for the pivot) and
// BinarySearch.Find over an OffsetSequence window (for the target) - the same shape
// SearchInRotatedSortedArraySolution (LC 33) uses, just offset into the trimmed
// slice. Worst case (e.g. an all-equal array) still degrades to O(n), matching the
// well-known result that duplicates rule out a guaranteed O(log n) solution here.
// HasTargetByLinearScan is the O(n) arm it has to beat regardless.
internal static class SearchInRotatedSortedArrayIISolution
{
    // The textbook O(n) scan. Written without this repo's own BinarySearch - the
    // arm HasTargetByTrimDuplicatesThenBinarySearch has to beat.
    public static bool HasTargetByLinearScan(int[] nums, int target) => Array.IndexOf(nums, target) >= 0;

    public static bool HasTargetByTrimDuplicatesThenBinarySearch(int[] nums, int target)
    {
        if (nums.Length == 0)
        {
            return false;
        }

        var range = TrimDuplicateBoundary(nums);
        var found = FindInTrimmedRange(nums, range, target);

        return found is not null;
    }

    private readonly record struct TrimmedRange(int Left, int Length, int LastValue);

    private static TrimmedRange TrimDuplicateBoundary(int[] nums)
    {
        var left = 0;
        var right = nums.Length - 1;

        while (left < right && nums[left] == nums[right])
        {
            left++;
        }

        return new TrimmedRange(left, right - left + 1, nums[right]);
    }

    private static int? FindInTrimmedRange(int[] nums, TrimmedRange range, int target)
    {
        var pivot = MonotonePredicateSearch.FirstTrue(
            0, range.Length - 1, new InTailRun(nums, range.Left, range.LastValue));
        var searchRight = target <= range.LastValue;
        var start = searchRight ? pivot : 0;
        var length = searchRight ? TailLength(range, pivot) : pivot;

        // The chosen half is a plain sorted window of nums, reindexed from 0 so
        // BinarySearch.Find can treat it like any other sorted sequence.
        return BinarySearch.Find<int, OffsetSequence<int>>(
            new OffsetSequence<int>(nums, range.Left + start, length), target);
    }

    // How many entries of the trimmed range lie past the pivot, in its tail run.
    private static int TailLength(TrimmedRange range, int pivot) => range.Length - pivot;

    // IsSatisfiedBy(offset) is true exactly when the trimmed slice's entry at that offset
    // belongs to its tail run - at or below the slice's last value: false before the
    // pivot and true from it on, so FirstTrue lands exactly on the pivot. The last
    // offset always holds, so the pivot always lies inside the slice.
    private readonly struct InTailRun(int[] nums, int start, int lastValue) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int offset) => nums[start + offset] <= lastValue;
    }
}
