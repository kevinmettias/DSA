namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInRotatedSortedArrayII;

public sealed class SearchInRotatedSortedArrayIITests
{
    [Theory]
    [InlineData(new[] { 2,5,6,0,0,1,2 }, 0, true)]
    [InlineData(new[] { 2,5,6,0,0,1,2 }, 3, false)]
    [InlineData(new[] { 1,0,1,1,1 }, 0, true)]
    public void Search_DuplicateAwareBinarySearch_ReturnsPresence(int[] nums, int target, bool expected) => Assert.Equal(expected, Search(nums, target));

    private static bool Search(int[] nums, int target)
    {
        var left = 0; var right = nums.Length - 1;
        while (left <= right)
        {
            var mid = left + (right - left) / 2;
            if (nums[mid] == target) return true;
            if (nums[left] == nums[mid] && nums[mid] == nums[right]) { left++; right--; }
            else if (nums[left] <= nums[mid]) { if (nums[left] <= target && target < nums[mid]) right = mid - 1; else left = mid + 1; }
            else { if (nums[mid] < target && target <= nums[right]) left = mid + 1; else right = mid - 1; }
        }
        return false;
    }
}
