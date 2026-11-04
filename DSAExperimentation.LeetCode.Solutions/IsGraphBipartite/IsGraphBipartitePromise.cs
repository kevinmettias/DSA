namespace DSAExperimentation.LeetCode.Solutions.IsGraphBipartite;

// LC 785's promise about its input: the graph is undirected: every neighbour is another node of the
// graph, listed once, and lists the node back. LeetCode's judge never poses an input that breaks
// it. An edge to a node past the last is read past the end of the strategies' colour arrays.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class IsGraphBipartitePromise
{
    public static bool IsKeptBy(int[][] graph)
        => Enumerable.Range(0, graph.Length).All(node => IsListedOnce(graph[node]) && graph[node].All(neighbor => IsEdge(graph, node, neighbor)));

    private static bool IsListedOnce(int[] neighbors) => neighbors.Distinct().Count() == neighbors.Length;

    // An edge to another node of the graph, which lists it back.
    private static bool IsEdge(int[][] graph, int node, int neighbor) => IsNodeOf(graph, neighbor) && IsListedBack(graph, node, neighbor);

    private static bool IsNodeOf(int[][] graph, int id) => id >= 0 && id < graph.Length;

    private static bool IsListedBack(int[][] graph, int node, int neighbor) => neighbor != node && graph[neighbor].Contains(node);
}
