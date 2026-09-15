namespace DSAExperimentation.LeetCode.MinimumTimeToVisitDisappearingNodes;

// LC 3112's edge list, adjacency-indexed once so both strategies below relax off
// an O(1) lookup per node instead of re-scanning the raw edges array.
internal sealed class TimedAdjacency
{
    public List<(int Neighbor, int Weight)>[] Neighbors { get; }

    private TimedAdjacency(List<(int Neighbor, int Weight)>[] neighbors) => Neighbors = neighbors;

    public static TimedAdjacency Build(int n, int[][] edges)
    {
        var neighbors = new List<(int Neighbor, int Weight)>[n];

        for (var i = 0; i < n; i++)
        {
            neighbors[i] = [];
        }

        foreach (var edge in edges)
        {
            var (u, v, weight) = (edge[0], edge[1], edge[2]);
            neighbors[u].Add((v, weight));
            neighbors[v].Add((u, weight));
        }

        return new TimedAdjacency(neighbors);
    }
}
