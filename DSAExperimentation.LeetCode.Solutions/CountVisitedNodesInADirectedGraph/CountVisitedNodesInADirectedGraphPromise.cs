namespace DSAExperimentation.LeetCode.Solutions.CountVisitedNodesInADirectedGraph;

// LC 2876's promise about its input: no node's edge leads back to itself, and each leads to a node
// of the graph. LeetCode's judge never poses an input that breaks it. The strongly connected
// components count a self-loop as no cycle, where the walk counts a cycle of one. Generated cases
// do not always keep the promise, so a harness that runs them asks this first.
internal static class CountVisitedNodesInADirectedGraphPromise
{
    public static bool IsKeptBy(int[] edges)
        => Enumerable.Range(0, edges.Length).All(node => edges[node] != node) && edges.All(target => target >= 0 && target < edges.Length);
}
