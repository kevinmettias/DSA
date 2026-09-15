namespace DSAExperimentation.LeetCode.MinimumWeightedSubgraphWithTheRequiredPaths;

// LC 2203's directed weighted graph, held in both orientations at once because the
// solution needs distances TO a fixed vertex as well as FROM two of them, and
// "distance to dest" is exactly "distance from dest on the edge-reversed graph".
// Both orientations are built in the single pass over LeetCode's own edges array
// that constructs the forward one, so the reverse copy costs nothing extra.
internal sealed class RequiredPathsGraph
{
    // Node i of each orientation is the same vertex i, so a candidate meeting
    // vertex is looked up by the same index in both.
    public RequiredPathsNode[] Forward { get; }

    public RequiredPathsNode[] Reverse { get; }

    private RequiredPathsGraph(RequiredPathsNode[] forward, RequiredPathsNode[] reverse)
    {
        Forward = forward;
        Reverse = reverse;
    }

    public static RequiredPathsGraph Build(int n, int[][] edges)
    {
        var forward = new RequiredPathsNode[n];
        var reverse = new RequiredPathsNode[n];

        for (var i = 0; i < n; i++)
        {
            forward[i] = new RequiredPathsNode(i);
            reverse[i] = new RequiredPathsNode(i);
        }

        foreach (var edge in edges)
        {
            var (from, to, weight) = (edge[0], edge[1], (long)edge[2]);
            forward[from].Edges.Add((weight, forward[to]));
            reverse[to].Edges.Add((weight, reverse[from]));
        }

        return new RequiredPathsGraph(forward, reverse);
    }
}
