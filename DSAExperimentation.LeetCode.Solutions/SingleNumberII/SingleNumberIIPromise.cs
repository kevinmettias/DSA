namespace DSAExperimentation.LeetCode.Solutions.SingleNumberII;

// LC 137's promise about its input: every number appears three times but one, which appears once.
// LeetCode's judge never poses an input that breaks it. Any other count leaves the bit counters
// holding a different number than the count mod three. Generated cases do not always keep the
// promise, so a harness that runs them asks this first.
internal static class SingleNumberIIPromise
{
    public static bool IsKeptBy(int[] nums)
    {
        var counts = nums.GroupBy(value => value).Select(group => group.Count()).ToArray();

        return counts.Count(count => count == 1) == 1 && counts.All(count => count is 1 or 3);
    }
}
