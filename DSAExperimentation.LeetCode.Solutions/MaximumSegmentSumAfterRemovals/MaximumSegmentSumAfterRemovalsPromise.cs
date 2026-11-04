namespace DSAExperimentation.LeetCode.Solutions.MaximumSegmentSumAfterRemovals;

// LC 2382's promise about its input: the queries remove every index once each. LeetCode's judge
// never poses an input that breaks it. A query past the last index is read past the end. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class MaximumSegmentSumAfterRemovalsPromise
{
    public static bool IsKeptBy(int[] nums, int[] removeQueries)
    {
        var everyIndex = Enumerable.Range(0, nums.Length);

        return removeQueries.Length == nums.Length && removeQueries.Order().SequenceEqual(everyIndex);
    }
}
