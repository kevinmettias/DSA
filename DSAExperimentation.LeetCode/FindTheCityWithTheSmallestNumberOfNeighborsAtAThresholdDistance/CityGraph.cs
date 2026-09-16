namespace DSAExperimentation.LeetCode.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// The full road network over all cityCount cities, built once from LeetCode's own
// edges array - the model, not an answer to any one query about it. Both strategies take one
// of these, so a benchmark can charge construction to [GlobalSetup] instead of
// to the search being measured (ARCHITECTURE.md 17.4).
internal sealed class CityGraph
{
    // Indexed by city id, so Cities[i].Id == i.
    public CityNode[] Cities { get; }

    private CityGraph(CityNode[] cities) => Cities = cities;

    // A CityNode per city id, then both directions of every road. LeetCodeAdjacency
    // states that layout once for every problem taking an (n, edges) pair; this
    // problem's own detail is that a slot holds the road's weight beside its far city,
    // which is what the wiring callback reads the edge's third value for.
    public static CityGraph Build(int cityCount, int[][] edges)
    {
        var cities = LeetCodeAdjacency.ZeroBased<CityNode>(
            cityCount, edges, id => new CityNode(id), (city, _, farCity, edgeIndex) => city.Edges.Add((edges[edgeIndex][2], farCity)));

        return new CityGraph(cities);
    }
}
