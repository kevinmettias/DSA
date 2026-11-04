namespace DSAExperimentation.LeetCode.Solutions.SingleNumber;

// LC 136's promise about its input: every number appears twice but one, which appears once.
// LeetCode's judge never poses an input that breaks it. With two numbers once each, the toggled set
// holds both. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class SingleNumberPromise
{
    public static bool IsKeptBy(int[] nums)
    {
        var counts = nums.GroupBy(value => value).Select(group => group.Count()).ToArray();

        return counts.Count(count => count == 1) == 1 && counts.All(count => count is 1 or 2);
    }
}
