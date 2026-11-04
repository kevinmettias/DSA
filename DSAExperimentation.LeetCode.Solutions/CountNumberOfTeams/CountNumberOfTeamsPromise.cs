namespace DSAExperimentation.LeetCode.Solutions.CountNumberOfTeams;

// LC 1395's promise about its input: every soldier's rating is unique. LeetCode's judge never poses
// an input that breaks it. Two equal ratings make a team neither increasing nor decreasing, which
// the strategies each count their own way. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class CountNumberOfTeamsPromise
{
    public static bool IsKeptBy(int[] rating) => rating.Distinct().Count() == rating.Length;
}
