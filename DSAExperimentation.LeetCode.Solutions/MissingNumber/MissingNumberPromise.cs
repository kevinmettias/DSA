namespace DSAExperimentation.LeetCode.Solutions.MissingNumber;

// LC 268's promise about its input: nums holds n distinct numbers from 0 to n. LeetCode's judge
// never poses an input that breaks it. A number past n leaves no single one missing. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class MissingNumberPromise
{
    public static bool IsKeptBy(int[] nums) => nums.All(value => value >= 0 && value <= nums.Length) && nums.Distinct().Count() == nums.Length;
}
