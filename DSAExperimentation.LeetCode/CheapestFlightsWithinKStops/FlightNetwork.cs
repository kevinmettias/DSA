namespace DSAExperimentation.LeetCode.CheapestFlightsWithinKStops;

// LC 787's flight list as a plain per-city adjacency list - no stop layers, no
// repo primitives, just the shape an unmemoized DFS walks. It exists so the naive
// baseline has a prepared-input overload of its own (ARCHITECTURE.md section
// 17.4): the benchmark builds this once in [GlobalSetup] instead of charging the
// adjacency build to every measured DFS.
internal sealed class FlightNetwork
{
    private readonly List<(int To, int Price)>[] _departures;

    private FlightNetwork(List<(int To, int Price)>[] departures) => _departures = departures;

    public static FlightNetwork Build(int n, int[][] flights)
    {
        var departures = new List<(int To, int Price)>[n];

        for (var city = 0; city < n; city++)
        {
            departures[city] = [];
        }

        foreach (var flight in flights)
        {
            departures[flight[0]].Add((flight[1], flight[2]));
        }

        return new FlightNetwork(departures);
    }

    public List<(int To, int Price)> DeparturesFrom(int city) => _departures[city];
}
