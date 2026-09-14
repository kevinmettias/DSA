namespace DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

// LC 1928's input as a plain per-city adjacency list plus its passing fees - no
// time expansion, no repo primitives, just the shape an unmemoized walk reads.
// Roads are two-way, so every edge row is recorded from both ends; a walk that
// only ever follows the rows in the order they were written would silently pass
// every one-directional example and fail the moment the cheap route runs
// backwards.
//
// It exists so the naive baseline has a prepared-input overload of its own
// (ARCHITECTURE.md section 17.4): the benchmark builds this once in [GlobalSetup]
// instead of charging the adjacency build to every measured walk.
internal sealed class RoadNetwork(List<(int To, int Time)>[] roads, int[] passingFees)
{
    // LeetCode states each road as one [from, to, minutes] row.
    private const int FromColumn = 0;
    private const int ToColumn = 1;
    private const int TimeColumn = 2;

    public int CityCount => roads.Length;

    // LeetCode always asks for the trip from city 0 to the last city.
    public int Destination => CityCount - 1;

    public static RoadNetwork Build(int[][] edges, int[] passingFees)
    {
        var roads = new List<(int To, int Time)>[passingFees.Length];

        for (var city = 0; city < roads.Length; city++)
        {
            roads[city] = [];
        }

        foreach (var edge in edges)
        {
            var (from, to, time) = (edge[FromColumn], edge[ToColumn], edge[TimeColumn]);

            roads[from].Add((to, time));
            roads[to].Add((from, time));
        }

        return new RoadNetwork(roads, passingFees);
    }

    public List<(int To, int Time)> RoadsFrom(int city) => roads[city];

    public int FeeAt(int city) => passingFees[city];
}
