
// One node per graph vertex; Edges holds the outgoing directed edges of whichever orientation
// of the input graph this node belongs to, weighted by long since a path's total cost can
// exceed int range (n and each weight are up to 10^5).
using RequiredPathsNode = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyNode<long>;

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

    public static RequiredPathsGraph Build(int nodeCount, int[][] edges)
    {
        var forward = new RequiredPathsNode[nodeCount];
        var reverse = new RequiredPathsNode[nodeCount];

        for (var i = 0; i < nodeCount; i++)
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
