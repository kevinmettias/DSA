namespace DSAExperimentation.LeetCode.MinimumTimeToVisitDisappearingNodes;

// LC 3112's edge list, adjacency-indexed once so both strategies below relax off
// an O(1) lookup per node instead of re-scanning the raw edges array.
internal sealed class TimedAdjacency
{
    public List<(int Neighbor, int Weight)>[] Neighbors { get; }

    private TimedAdjacency(List<(int Neighbor, int Weight)>[] neighbors) => Neighbors = neighbors;

    public static TimedAdjacency Build(int nodeCount, int[][] edges)
    {
        var neighbors = LeetCodeAdjacency.ZeroBased<List<(int To, int Weight)>, WeightedNeighborSlots<int>>(nodeCount, edges, new WeightedNeighborSlots<int>(edges));

        return new TimedAdjacency(neighbors);
    }
}
