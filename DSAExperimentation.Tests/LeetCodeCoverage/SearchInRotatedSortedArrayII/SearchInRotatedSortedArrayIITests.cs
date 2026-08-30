using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInRotatedSortedArrayII;

// LeetCode 81. Search in Rotated Sorted Array II: duplicates can make the rotation
// boundary ambiguous (nums[left] == nums[right] doesn't say which side the pivot
// is on), so this first trims matching values off the left edge - correct for a
// presence-only search because trimming nums[left] only ever happens while an
// equal-valued witness still sits at nums[right], so no value's last remaining
// occurrence is ever trimmed away. Once nums[left] != nums[right] (or the range
// collapses to one element), the remaining slice has no duplicate-boundary
// ambiguity left, so it's handed to this repo's own BinarySearch.LowerBound/Find
// over the same PivotSequence/OffsetSequence witness shape SearchInRotatedSortedArrayTests
// (LC 33) already uses, just offset into the trimmed slice - unmodified otherwise.
// Worst case (e.g. an all-equal array) still degrades to O(n), matching the
// well-known result that duplicates rule out a guaranteed O(log n) solution here.
public sealed partial class SearchInRotatedSortedArrayIITests
{
    [Theory]
    [InlineData(new[] { 2, 5, 6, 0, 0, 1, 2 }, 0, true)]
    [InlineData(new[] { 2, 5, 6, 0, 0, 1, 2 }, 3, false)]
    [InlineData(new[] { 1, 0, 1, 1, 1 }, 0, true)]
    [InlineData(new[] { 1, 1, 1, 1, 1 }, 2, false)]
    [InlineData(new int[] { }, 5, false)]
    public void Search_TrimDuplicatesThenBinarySearch_ReturnsPresence(int[] nums, int target, bool expected)
        => Assert.Equal(expected, Search(nums, target));

    private static bool Search(int[] nums, int target)
    {
        if (nums.Length == 0)
        {
            return false;
        }

        var left = 0;
        var right = nums.Length - 1;

        while (left < right && nums[left] == nums[right])
        {
            left++;
        }

        var trimmedLength = right - left + 1;
        var trimmedLast = nums[right];

        var pivot = BinarySearch.LowerBound<int, PivotSequence>(new PivotSequence(nums, left, trimmedLength, trimmedLast), 1);
        var searchRight = target <= trimmedLast;
        var start = searchRight ? pivot : 0;
        var length = searchRight ? trimmedLength - pivot : pivot;
        var found = BinarySearch.Find<int, OffsetSequence>(new OffsetSequence(nums, left + start, length), target);

        return found is not null;
    }

    private readonly struct PivotSequence(int[] nums, int start, int length, int lastValue) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int index) => nums[start + index] <= lastValue ? 1 : 0;
    }

    private readonly struct OffsetSequence(int[] nums, int start, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int index) => nums[start + index];
    }
}
