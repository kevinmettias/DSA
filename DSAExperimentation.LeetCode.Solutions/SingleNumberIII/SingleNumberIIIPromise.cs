namespace DSAExperimentation.LeetCode.Solutions.SingleNumberIII;

// LC 260's promise about its input: every number appears twice but two, which appear once each.
// LeetCode's judge never poses an input that breaks it. Any other count leaves the strategies a
// different pair. Generated cases do not always keep the promise, so a harness that runs them asks
// this first.
internal static class SingleNumberIIIPromise
{
    public static bool IsKeptBy(int[] nums)
    {
        var counts = nums.GroupBy(value => value).Select(group => group.Count()).ToArray();

        return counts.Count(count => count == 1) == 2 && counts.All(count => count is 1 or 2);
    }
}
