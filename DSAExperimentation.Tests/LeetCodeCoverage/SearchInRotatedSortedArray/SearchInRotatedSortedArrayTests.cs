using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInRotatedSortedArray;

public sealed partial class SearchInRotatedSortedArrayTests
{
    [Theory]
    [InlineData(new[] { 4, 5, 6, 7, 0, 1, 2 }, 0, 4)]
    [InlineData(new[] { 4, 5, 6, 7, 0, 1, 2 }, 3, -1)]
    [InlineData(new[] { 1 }, 0, -1)]
    public void Search_LeetCodeExamples_ReturnsTargetIndexOrMinusOne(int[] nums, int target, int expected)
        => Assert.Equal(expected, Search(nums, target));

    private static int Search(int[] nums, int target)
    {
        if (nums.Length == 0) return -1;
        var pivot = BinarySearch.LowerBound<int, PivotSequence>(new PivotSequence(nums), 1);
        var searchRight = target <= nums[^1];
        var start = searchRight ? pivot : 0;
        var length = searchRight ? nums.Length - pivot : pivot;
        var found = BinarySearch.Find<int, OffsetSequence>(new OffsetSequence(nums, start, length), target);
        return found is null ? -1 : start + found.Value;
    }

    private readonly struct PivotSequence(int[] nums) : IRandomAccessSequence<int>
    {
        public int Length => nums.Length;
        public int Get(int index) => nums[index] <= nums[^1] ? 1 : 0;
    }

    private readonly struct OffsetSequence(int[] nums, int start, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int index) => nums[start + index];
    }
}
