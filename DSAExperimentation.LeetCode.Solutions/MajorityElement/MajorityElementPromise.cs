namespace DSAExperimentation.LeetCode.Solutions.MajorityElement;

// LC 169's promise about its input: some element appears more than n / 2 times. LeetCode's judge
// never poses an input that breaks it. Without one, the vote and the count each answer with a
// different element. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class MajorityElementPromise
{
    public static bool IsKeptBy(int[] nums) => nums.GroupBy(value => value).Max(group => group.Count()) * 2 > nums.Length;
}
