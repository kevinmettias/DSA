namespace DSAExperimentation.LeetCode.Solutions.QueryKthSmallestTrimmedNumber;

// LC 2343's promise about its input: every number has one length, and every query asks a rank and a
// trim inside it. LeetCode's judge never poses an input that breaks it. A trim past a number's
// length is read past its start. Generated cases do not always keep the promise, so a harness that
// runs them asks this first.
internal static class QueryKthSmallestTrimmedNumberPromise
{
    public static bool IsKeptBy(string[] nums, int[][] queries)
        => nums.All(number => number.Length == nums[0].Length) && queries.All(query => IsInside(query, nums));

    private static bool IsInside(int[] query, string[] nums)
        => query[0] >= 1 && query[0] <= nums.Length && query[1] >= 1 && query[1] <= nums[0].Length;
}
