namespace DSAExperimentation.Tests.LeetCodeCoverage.CheapestFlightsWithinKStops.Fixtures;

// One node per (city, edges-used-to-reach-it) pair: "at most K stops" is really
// "at most K+1 edges," a constraint ShortestPath.Dijkstra has no parameter for -
// so it's baked into the graph's own shape instead. An edge only ever moves a
// state from stop layer L to L+1, so no path through this graph can ever use more
// edges than its layer count allows, and Dijkstra needs no modification at all.
internal sealed class FlightState(int city, int stops)
{
    public int City { get; } = city;

    public int Stops { get; } = stops;

    public List<(int Price, FlightState Target)> Edges { get; } = [];

    public override string ToString() => $"{City}@{Stops}";
}
