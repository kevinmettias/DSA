namespace DSAExperimentation.LeetCode.Solutions.MakeArrayEmpty;

// LC 2659's promise about its input: all the values are distinct. LeetCode's judge never poses an
// input that breaks it. Equal values have no single order of removal, and the strategies each pick
// their own. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class MakeArrayEmptyPromise
{
    public static bool IsKeptBy(int[] nums) => nums.Distinct().Count() == nums.Length;
}
