namespace DSAExperimentation.LeetCode.Solutions.FindEventualSafeStates;

// LC 802's promise about its input: every edge leads to a node of the graph, 0 to n - 1. LeetCode's
// judge never poses an input that breaks it. An edge to a node past the last is read past the end
// of the strategies' arrays. Generated cases do not always keep the promise, so a harness that runs
// them asks this first.
internal static class FindEventualSafeStatesPromise
{
    public static bool IsKeptBy(int[][] graph) => graph.All(edges => edges.All(node => node >= 0 && node < graph.Length));
}
