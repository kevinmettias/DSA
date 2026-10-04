namespace DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

// LC 1928's input as a plain per-city adjacency list plus its passing fees - no
// time expansion and no repo container, just the BCL lists an unmemoized walk reads,
// laid out by LeetCodeAdjacency (section 17.5). Each list pairs a neighbour with the
// road's minutes, LeetCode's third column. Roads are two-way, so every edge row is
// recorded from both ends; a walk that
// only ever follows the rows in the order they were written would silently pass
// every one-directional example and fail the moment the cheap route runs
// backwards.
//
// It exists so the naive baseline has a prepared-input overload of its own
// (ARCHITECTURE.md section 17.4): the benchmark builds this once in [GlobalSetup]
// instead of charging the adjacency build to every measured walk.
internal sealed class RoadNetwork(List<(int To, int Time)>[] roads, int[] passingFees)
{
    public int CityCount => roads.Length;

    // LeetCode always asks for the trip from city 0 to the last city.
    public int Destination => CityCount - 1;

    public static RoadNetwork Build(int[][] edges, int[] passingFees)
    {
        var roads = LeetCodeAdjacency.ZeroBased<List<(int To, int Weight)>, WeightedNeighborSlots<int>>(passingFees.Length, edges, new WeightedNeighborSlots<int>(edges));

        return new RoadNetwork(roads, passingFees);
    }

    public List<(int To, int Time)> RoadsFrom(int city) => roads[city];

    public int FeeAt(int city) => passingFees[city];
}
