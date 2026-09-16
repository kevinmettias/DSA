namespace DSAExperimentation.LeetCode.MinimizeTheMaximumEdgeWeightOfGraph;

// LC 3419's own graph, reversed at construction time: an original edge A -> B
// becomes an adjacency entry under B, so a walk rooted at node 0 explores exactly
// the nodes that can reach node 0 in the original graph - the same reversal
// GridShortestPath's caller never needs, because a grid's edges are already
// symmetric. Meaningless outside this one problem, so it stays in this folder
// rather than Domain/ (ARCHITECTURE.md §17.3).
internal sealed class EdgeWeightGraph(List<(int To, int Weight)>[] reversedAdjacency, int maxWeight)
{
    public int NodeCount => reversedAdjacency.Length;

    public int MaxWeight => maxWeight;

    public IReadOnlyList<(int To, int Weight)> NeighborsOf(int node) => reversedAdjacency[node];

    public static EdgeWeightGraph Build(int nodeCount, int[][] edges)
    {
        var reversedAdjacency = new List<(int To, int Weight)>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            reversedAdjacency[i] = [];
        }

        var maxWeight = 0;

        foreach (var edge in edges)
        {
            var (from, to, weight) = (edge[0], edge[1], edge[2]);
            reversedAdjacency[to].Add((from, weight));
            maxWeight = Math.Max(maxWeight, weight);
        }

        return new EdgeWeightGraph(reversedAdjacency, maxWeight);
    }
}
