namespace DSAExperimentation.LeetCode.Solutions.FindTheDuplicateNumber;

// LC 287's promise about its input: nums holds n + 1 integers, each from 1 to n, and exactly one of
// them repeats. LeetCode's judge never poses an input that breaks it. The cycle detection follows
// each value as an index, so a value past n leaves the array. Generated cases do not always keep
// the promise, so a harness that runs them asks this first.
internal static class FindTheDuplicateNumberPromise
{
    public static bool IsKeptBy(int[] nums)
    {
        var largest = nums.Length - 1;
        var repeated = nums.GroupBy(value => value).Count(group => group.Count() > 1);

        return nums.All(value => value >= 1 && value <= largest) && repeated == 1;
    }
}
