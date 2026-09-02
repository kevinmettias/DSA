namespace DSAExperimentation.LeetCode.NetworkRecoveryPathways;

// The graph LC 3620 actually poses a query over, online-filtered once: any edge
// touching an offline node can never lie on a valid path (0 and n-1 are always
// online per the problem's own constraint), so dropping such edges up front is
// exactly "every intermediate node on the path is online" - the domain model,
// not an answer to any one query about it (LockGraph's own framing). Both
// strategies below share this same prepared network; only how each one searches
// it differs.
internal sealed class RecoveryNetwork
{
    private RecoveryNetwork(RecoveryNode[] nodes, List<(int From, int To, long Weight)> onlineEdges, long maxCost)
    {
        Nodes = nodes;
        OnlineEdges = onlineEdges;
        MaxCost = maxCost;
    }

    public RecoveryNode[] Nodes { get; }

    public List<(int From, int To, long Weight)> OnlineEdges { get; }

    // Upper bound for the path-score binary search: no path's minimum edge can
    // exceed the heaviest online edge in the graph.
    public long MaxCost { get; }

    public RecoveryNode Source => Nodes[0];

    public RecoveryNode Destination => Nodes[^1];

    public static RecoveryNetwork Build(int n, int[][] edges, bool[] online)
    {
        var nodes = new RecoveryNode[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new RecoveryNode(i);
        }

        var onlineEdges = new List<(int From, int To, long Weight)>();
        var maxCost = 0L;

        foreach (var edge in edges)
        {
            var from = edge[0];
            var to = edge[1];
            var weight = (long)edge[2];

            if (!online[from] || !online[to])
            {
                continue;
            }

            onlineEdges.Add((from, to, weight));
            maxCost = Math.Max(maxCost, weight);
        }

        return new RecoveryNetwork(nodes, onlineEdges, maxCost);
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
