namespace DSAExperimentation.LeetCode.Solutions.SearchInRotatedSortedArray;

// LC 33's promise about its input: nums is a strictly increasing array rotated, so read around the
// end it descends exactly once. LeetCode's judge never poses an input that breaks it. The pivot
// search finds where the rotation descends, which an array descending twice does not have.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class SearchInRotatedSortedArrayPromise
{
    public static bool IsKeptBy(int[] nums, int target) => nums.Length == 1 || DescentsAround(nums) == 1;

    // Each place, reading around the end back to the start, where the next value is not larger.
    private static int DescentsAround(int[] nums)
    {
        var descents = 0;

        for (var index = 0; index < nums.Length; index++)
        {
            if (nums[index] >= nums[(index + 1) % nums.Length])
            {
                descents++;
            }
        }

        return descents;
    }
}
