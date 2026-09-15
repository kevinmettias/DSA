using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SearchInRotatedSortedArrayII;

// LeetCode 81. Search in Rotated Sorted Array II: report whether target is
// present in an ascending array that has been rotated at some unknown pivot and
// may contain duplicates.
//
// Duplicates can make the rotation boundary ambiguous (nums[left] == nums[right]
// doesn't say which side the pivot is on), so SearchByTrimDuplicatesThenBinarySearch
// first trims matching values off the left edge - correct for a presence-only
// search because trimming nums[left] only ever happens while an equal-valued
// witness still sits at nums[right], so no value's last remaining occurrence is
// ever trimmed away. Once nums[left] != nums[right] (or the range collapses to one
// element), the remaining slice has no duplicate-boundary ambiguity left, so it is
// handed to this repo's own BinarySearch.LowerBound/Find over the same
// PivotSequence/OffsetSequence shape SearchInRotatedSortedArraySolution (LC 33)
// uses, just offset into the trimmed slice. Worst case (e.g. an all-equal array)
// still degrades to O(n), matching the well-known result that duplicates rule out
// a guaranteed O(log n) solution here. SearchByLinearScan is the O(n) arm it has
// to beat regardless.
internal static class SearchInRotatedSortedArrayIISolution
{
    // The textbook O(n) scan. Written without this repo's own BinarySearch - the
    // arm TrimDuplicatesThenBinarySearch has to beat.
    public static bool SearchByLinearScan(int[] nums, int target) => Array.IndexOf(nums, target) >= 0;

    public static bool SearchByTrimDuplicatesThenBinarySearch(int[] nums, int target)
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
        var pivot = BinarySearch.LowerBound<int, PivotSequence>(
            new PivotSequence(nums, range.Left, range.Length, range.LastValue), 1);
        var searchRight = target <= range.LastValue;
        var start = searchRight ? pivot : 0;
        var length = searchRight ? TailLength(range, pivot) : pivot;

        return BinarySearch.Find<int, OffsetSequence>(new OffsetSequence(nums, range.Left + start, length), target);
    }

    // How many entries of the trimmed range lie past the pivot, in its tail run.
    private static int TailLength(TrimmedRange range, int pivot) => range.Length - pivot;

    // 1 past the pivot, 0 up to and including it: LowerBound on this predicate
    // lands exactly on the first index belonging to the trimmed slice's tail run.
    private readonly struct PivotSequence(int[] nums, int start, int length, int lastValue) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int index) => IsInTailRun(index) ? 1 : 0;

        private bool IsInTailRun(int index) => nums[start + index] <= lastValue;
    }

    // A plain sorted window [start, start + length) of nums, reindexed from 0 so
    // BinarySearch.Find can treat it like any other sorted sequence.
    private readonly struct OffsetSequence(int[] nums, int start, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int index) => nums[start + index];
    }
}
