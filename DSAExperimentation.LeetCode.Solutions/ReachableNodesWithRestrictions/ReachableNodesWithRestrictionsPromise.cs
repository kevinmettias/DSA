namespace DSAExperimentation.LeetCode.Solutions.ReachableNodesWithRestrictions;

// LC 2368's promise about its input: the restricted nodes are unique and never node 0, where the
// walk starts. LeetCode's judge never poses an input that breaks it. With node 0 restricted the
// flood fill still counts it and the disjoint set does not. Generated cases do not always keep the
// promise, so a harness that runs them asks this first.
internal static class ReachableNodesWithRestrictionsPromise
{
    public static bool IsKeptBy(int nodeCount, int[][] edges, int[] restricted)
        => restricted.All(node => node >= 1 && node < nodeCount) && restricted.Distinct().Count() == restricted.Length;
}
