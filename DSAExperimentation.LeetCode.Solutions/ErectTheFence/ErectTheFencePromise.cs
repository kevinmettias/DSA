namespace DSAExperimentation.LeetCode.Solutions.ErectTheFence;

// LC 587's promise about its input: all the given positions are unique. LeetCode's judge never poses
// an input that breaks it. Both strategies answer each fence post once, where a repeated tree would
// stand twice in the answer. Generated cases do not always keep the promise, so a harness that runs
// them asks this first.
internal static class ErectTheFencePromise
{
    public static bool IsKeptBy((int X, int Y)[] points) => points.Distinct().Count() == points.Length;
}
