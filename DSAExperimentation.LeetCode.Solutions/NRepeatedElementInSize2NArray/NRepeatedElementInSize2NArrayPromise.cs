namespace DSAExperimentation.LeetCode.Solutions.NRepeatedElementInSize2NArray;

// LC 961's promise about its input: nums holds 2n numbers, n + 1 of them unique, one of those
// repeated n times. LeetCode's judge never poses an input that breaks it. With two elements
// repeated, the pairwise scan and the set each find a different one first. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class NRepeatedElementInSize2NArrayPromise
{
    public static bool IsKeptBy(int[] nums)
    {
        var half = nums.Length / 2;
        var counts = nums.GroupBy(value => value).Select(group => group.Count()).ToArray();

        return nums.Length % 2 == 0 && IsOneRepeatedHalfTheTimes(counts, half);
    }

    private static bool IsOneRepeatedHalfTheTimes(int[] counts, int half) => counts.Length == half + 1 && counts.Contains(half);
}
