namespace DSAExperimentation.LeetCode.Solutions.TwoSum;

// LC 1's promise about its input: exactly one pair of distinct indices sums to the target. LeetCode's
// judge never poses an input that breaks it, and the strategies lean on it - each answers with the
// first pair it meets, and the two meet pairs in different orders - so an input with several pairs
// has several right answers and the strategies may each give a different one. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class TwoSumPromise
{
    public static bool IsKeptBy(int[] nums, int target) => PairCount(nums, target) == 1;

    private static int PairCount(int[] nums, int target)
    {
        var count = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] + nums[j] == target)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
