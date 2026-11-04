namespace DSAExperimentation.LeetCode.Solutions.SortedGcdPairQueries;

// LC 3312's promise about its input: every query names one of the n * (n - 1) / 2 pairs. LeetCode's
// judge never poses an input that breaks it. A query past the last pair is read past the end of the
// brute force's list. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class SortedGcdPairQueriesPromise
{
    public static bool IsKeptBy(int[] nums, int[] queries)
    {
        var pairCount = (long)nums.Length * (nums.Length - 1) / 2;

        return queries.All(query => query >= 0 && query < pairCount);
    }
}
