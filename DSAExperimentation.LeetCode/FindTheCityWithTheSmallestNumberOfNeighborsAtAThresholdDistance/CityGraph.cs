namespace DSAExperimentation.LeetCode.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// The full n-city road network, built once from LeetCode's own edges array -
// the model, not an answer to any one query about it. Both strategies take one
// of these, so a benchmark can charge construction to [GlobalSetup] instead of
// to the search being measured (ARCHITECTURE.md 17.4).
internal sealed class CityGraph
{
    // Indexed by city id, so Cities[i].Id == i.
    public CityNode[] Cities { get; }

    private CityGraph(CityNode[] cities) => Cities = cities;

    public static CityGraph Build(int n, int[][] edges)
    {
        var cities = new CityNode[n];

        for (var i = 0; i < n; i++)
        {
            cities[i] = new CityNode(i);
        }

        foreach (var edge in edges)
        {
            var (from, to, weight) = (edge[0], edge[1], edge[2]);
            cities[from].Edges.Add((weight, cities[to]));
            cities[to].Edges.Add((weight, cities[from]));
        }

        return new CityGraph(cities);
    }
}
