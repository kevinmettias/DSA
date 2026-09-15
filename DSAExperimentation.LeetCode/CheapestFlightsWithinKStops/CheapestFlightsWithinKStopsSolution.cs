using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.CheapestFlightsWithinKStops;

// LeetCode 787. Cheapest Flights Within K Stops: the cheapest price from src to
// dst using at most K intermediate stops, or -1 when no such itinerary exists.
//
// Dijkstra has no "at most this many edges" parameter, so the K-stop limit is
// expressed as graph shape instead of algorithm logic - FlightStateGraph's layered
// (city, edgesUsed) state space - and ShortestPath.Dijkstra runs over that
// expanded graph completely unmodified. Flight prices are non-negative, exactly
// Dijkstra's own precondition. FindCheapestPriceByNaiveDfs is the textbook
// alternative it has to justify itself against: every path with at most K+1 edges,
// unmemoized and exponential in K.
internal static class CheapestFlightsWithinKStopsSolution
{
    // The textbook answer: recurse over every itinerary with edges to spare,
    // keeping the cheapest arrival at dst. Deliberately written without this
    // repo's primitives - only the adjacency container it is handed is a repo
    // type (ARCHITECTURE.md section 17.5).
    public static int FindCheapestPriceByNaiveDfs(
        int n, int[][] flights, (int Source, int Destination) endpoints, int k)
    {
        var network = FlightNetwork.Build(n, flights);

        return FindCheapestPriceByNaiveDfs(network, endpoints.Source, endpoints.Destination, k);
    }

    public static int FindCheapestPriceByNaiveDfs(FlightNetwork network, int src, int dst, int k)
    {
        var best = int.MaxValue;

        // "At most K stops" is "at most K+1 edges" - that is the DFS's depth budget.
        Explore(new FlightSearch(network, dst), (City: src, EdgesLeft: k + 1, CostSoFar: 0), ref best);

        return best == int.MaxValue ? LeetCodeAnswer.None : best;
    }

    // One recursive step's position in the walk: the city it stands in, the edge
    // budget it has left, and what the itinerary has cost so far - three values
    // that only ever move together, one step to the next.
    private static void Explore(
        FlightSearch search, (int City, int EdgesLeft, int CostSoFar) walk, ref int best)
    {
        var (city, edgesLeft, costSoFar) = walk;

        if (city == search.Destination && costSoFar < best)
        {
            best = costSoFar;
        }

        if (edgesLeft == 0)
        {
            return;
        }

        foreach (var (to, price) in search.Network.DeparturesFrom(city))
        {
            Explore(
                search,
                (City: to, EdgesLeft: edgesLeft - 1, CostSoFar: costSoFar + price),
                ref best);
        }
    }

    // Bundles the parts of the walk that never change, so a recursive step names
    // one search parameter instead of the adjacency and the destination.
    private readonly record struct FlightSearch(FlightNetwork Network, int Destination);

    // This repo's own Dijkstra, run over the stop-layered expansion: an edge only
    // ever moves a state from layer L to L+1, so every path the search can find
    // already respects the stop bound and no K-aware logic is needed. The answer
    // is the cheapest arrival at dst in any layer, since finishing early is always
    // allowed.
    public static int FindCheapestPriceByDijkstraOverStopLayers(
        int n, int[][] flights, (int Source, int Destination) endpoints, int k)
    {
        var graph = FlightStateGraph.Build(n, flights, k);

        return FindCheapestPriceByDijkstraOverStopLayers(graph, endpoints.Source, endpoints.Destination);
    }

    public static int FindCheapestPriceByDijkstraOverStopLayers(FlightStateGraph graph, int src, int dst)
    {
        var source = graph.At(src, 0);

        var distances = ShortestPath.Dijkstra<
            FlightState, FlightStateTopology, ListEdges<FlightState, int>, int>(source);

        var best = CheapestArrival(distances, graph, dst);

        return best == int.MaxValue ? LeetCodeAnswer.None : best;
    }

    private static int CheapestArrival(
        Dictionary<FlightState, int> distances, FlightStateGraph graph, int dst)
    {
        var best = int.MaxValue;

        for (var layer = 0; layer <= graph.MaxEdges; layer++)
        {
            var arrival = graph.At(dst, layer);

            if (distances.TryGetValue(arrival, out var price) && price < best)
            {
                best = price;
            }
        }

        return best;
    }
}
