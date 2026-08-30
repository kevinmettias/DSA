using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReconstructItinerary;

// LeetCode 332. Reconstruct Itinerary: Hierholzer's algorithm over an adjacency map
// built from this repo's own HashMap<TKey,TValue>, keyed by departure airport, whose
// values are a Heap<Element,TOrder> closed over MinHeapOrder<string> - always pops
// the lexicographically smallest unused destination first. DFS consumes exactly one
// heap entry (one ticket) per step and only appends a stop to the route once it's a
// dead end (nothing left to pop), which is what turns a plain DFS into Hierholzer's:
// the route comes out in reverse post-order.
public sealed partial class ReconstructItineraryTests
{
    [Fact]
    public void FindItinerary_SingleValidRoute_ReturnsStopsInVisitedOrder()
    {
        string[][] tickets =
        [
            ["MUC", "LHR"],
            ["JFK", "MUC"],
            ["SFO", "SJC"],
            ["LHR", "SFO"],
        ];

        var itinerary = FindItinerary(tickets);

        Assert.Equal(["JFK", "MUC", "LHR", "SFO", "SJC"], itinerary);
    }

    [Fact]
    public void FindItinerary_MultipleValidRoutes_PrefersLexicallySmallerPath()
    {
        string[][] tickets =
        [
            ["JFK", "SFO"],
            ["JFK", "ATL"],
            ["SFO", "ATL"],
            ["ATL", "JFK"],
            ["ATL", "SFO"],
        ];

        var itinerary = FindItinerary(tickets);

        Assert.Equal(["JFK", "ATL", "JFK", "SFO", "ATL", "SFO"], itinerary);
    }

    private static List<string> FindItinerary(string[][] tickets)
    {
        var graph = new HashMap<string, Heap<string, MinHeapOrder<string>>>();

        foreach (var ticket in tickets)
        {
            if (!graph.TryGetValue(ticket[0], out var destinations))
            {
                destinations = new Heap<string, MinHeapOrder<string>>();
                graph.Set(ticket[0], destinations);
            }

            destinations.Push(ticket[1]);
        }

        var route = new List<string>();
        Visit("JFK", graph, route);
        route.Reverse();
        return route;
    }

    private static void Visit(
        string airport, HashMap<string, Heap<string, MinHeapOrder<string>>> graph, List<string> route)
    {
        if (graph.TryGetValue(airport, out var destinations))
        {
            while (destinations.TryPop(out var next))
            {
                Visit(next, graph, route);
            }
        }

        route.Add(airport);
    }
}
