namespace DSAExperimentation.LeetCode.MinimumTimeToReachDestinationInDirectedGraph;

// LC 3604's directed, time-windowed edge list, adjacency-indexed once so both
// strategies below relax off an O(1) lookup per node instead of re-scanning the
// raw edges array - the same shape MinimumTimeToVisitDisappearingNodes's own
// TimedAdjacency uses for LC 3112's undirected, weight-only edges.
internal sealed class TimeWindowAdjacency
{
    private TimeWindowAdjacency(List<(int Neighbor, int Start, int End)>[] neighbors) => Neighbors = neighbors;

    public List<(int Neighbor, int Start, int End)>[] Neighbors { get; }

    public static TimeWindowAdjacency Build(int n, int[][] edges)
    {
        var neighbors = new List<(int Neighbor, int Start, int End)>[n];

        for (var i = 0; i < n; i++)
        {
            neighbors[i] = [];
        }

        foreach (var edge in edges)
        {
            var (u, v, start, end) = (edge[0], edge[1], edge[2], edge[3]);
            neighbors[u].Add((v, start, end));
        }

        return new TimeWindowAdjacency(neighbors);
    }
}
