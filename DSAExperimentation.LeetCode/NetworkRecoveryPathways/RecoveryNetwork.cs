namespace DSAExperimentation.LeetCode.NetworkRecoveryPathways;

// The graph LC 3620 actually poses a query over, online-filtered once: any edge
// touching an offline node can never lie on a valid path (0 and nodeCount - 1 are
// always online per the problem's own constraint), so dropping such edges up front is
// exactly "every intermediate node on the path is online" - the domain model,
// not an answer to any one query about it (LockGraph's own framing). Both
// strategies below share this same prepared network; only how each one searches
// it differs.
internal sealed class RecoveryNetwork(
    RecoveryNode[] nodes, List<(int From, int To, long Weight)> onlineEdges, long maxCost)
{
    // The three views RecoveryNetworkPathwaysSolution reads by name, kept as
    // properties over the primary constructor's own values.
    public RecoveryNode[] Nodes => nodes;

    public List<(int From, int To, long Weight)> OnlineEdges => onlineEdges;

    // Upper bound for the path-score binary search: no path's minimum edge can
    // exceed the heaviest online edge in the graph.
    public long MaxCost => maxCost;

    public RecoveryNode Source => Nodes[0];

    public RecoveryNode Destination => Nodes[^1];

    public static RecoveryNetwork Build(int nodeCount, int[][] edges, bool[] online)
    {
        var nodes = BuildNodes(nodeCount);
        var onlineEdges = new List<(int From, int To, long Weight)>();
        var maxCost = 0L;

        foreach (var edge in edges)
        {
            maxCost = AdmitEdge(edge, online, onlineEdges, maxCost);
        }

        return new RecoveryNetwork(nodes, onlineEdges, maxCost);
    }

    private static RecoveryNode[] BuildNodes(int nodeCount)
    {
        var nodes = new RecoveryNode[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i] = new RecoveryNode(i);
        }

        return nodes;
    }

    // An edge with an offline endpoint can never lie on a valid path, so it is dropped;
    // an admitted edge's weight feeds the running upper bound for the path-score search.
    private static long AdmitEdge(
        int[] edge, bool[] online, List<(int From, int To, long Weight)> onlineEdges, long maxCost)
    {
        var from = edge[0];
        var to = edge[1];
        var weight = (long)edge[2];

        if (!online[from] || !online[to])
        {
            return maxCost;
        }

        onlineEdges.Add((from, to, weight));

        return Math.Max(maxCost, weight);
    }

    // Repopulates every node's outgoing edges to exactly the online edges whose
    // cost clears `threshold` - the graph a binary-search probe at that
    // threshold has to search, rebuilt from OnlineEdges rather than re-filtered
    // from the raw LeetCode input each time.
    public void Rebuild(long threshold)
    {
        foreach (var node in Nodes)
        {
            node.Edges.Clear();
        }

        foreach (var (from, to, weight) in OnlineEdges)
        {
            if (weight >= threshold)
            {
                Nodes[from].Edges.Add((weight, Nodes[to]));
            }
        }
    }
}
