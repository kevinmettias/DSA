namespace DSAExperimentation.LeetCode.Solutions.MostFrequentIds;

// LC 3092's promise about its input: no step takes an ID below zero occurrences, and every step
// changes one. LeetCode's judge never poses an input that breaks it. A negative count is no
// frequency, and the strategies floor it differently. Generated cases do not always keep the
// promise, so a harness that runs them asks this first.
internal static class MostFrequentIdsPromise
{
    public static bool IsKeptBy(int[] nums, int[] freq)
    {
        var counts = new Dictionary<int, long>();

        for (var step = 0; step < nums.Length; step++)
        {
            var count = counts.GetValueOrDefault(nums[step]) + freq[step];
            counts[nums[step]] = count;

            if (count < 0 || freq[step] == 0)
            {
                return false;
            }
        }

        return nums.Length == freq.Length;
    }
}
