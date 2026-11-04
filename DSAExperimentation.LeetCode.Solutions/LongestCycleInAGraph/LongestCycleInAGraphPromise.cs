namespace DSAExperimentation.LeetCode.Solutions.LongestCycleInAGraph;

// LC 2360's promise about its input: no node's edge leads back to itself, and each leads to a node
// of the graph or nowhere. LeetCode's judge never poses an input that breaks it. The strongly
// connected components count a self-loop as no cycle, where the walk counts a cycle of one.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class LongestCycleInAGraphPromise
{
    public static bool IsKeptBy(int[] edges)
        => Enumerable.Range(0, edges.Length).All(node => edges[node] != node) && edges.All(target => target >= -1 && target < edges.Length);
}
