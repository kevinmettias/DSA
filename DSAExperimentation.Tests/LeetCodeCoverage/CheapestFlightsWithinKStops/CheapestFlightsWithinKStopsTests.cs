using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.CheapestFlightsWithinKStops.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheapestFlightsWithinKStops;

// LeetCode 787. Cheapest Flights Within K Stops: this repo's own ShortestPath.
// Dijkstra has no "at most this many edges" parameter, so the K-stop limit is
// expressed as graph shape instead of algorithm logic - a layered (city,
// edgesUsed) state space (see Fixtures.FlightState) - and Dijkstra runs over that
// expanded graph completely unmodified. Flight prices are non-negative, exactly
// Dijkstra's own precondition.
public sealed partial class CheapestFlightsWithinKStopsTests
{
    [Fact]
    public void FindCheapestPrice_OneStopAllowed_ReturnsCheaperTwoHopPrice()
    {
        int[][] flights = [[0, 1, 100], [1, 2, 100], [0, 2, 500]];

        var price = FindCheapestPrice(n: 3, flights, src: 0, dst: 2, k: 1);

        Assert.Equal(200, price);
    }

    [Fact]
    public void FindCheapestPrice_ZeroStopsAllowed_ReturnsDirectFlightPrice()
    {
        int[][] flights = [[0, 1, 100], [1, 2, 100], [0, 2, 500]];

        var price = FindCheapestPrice(n: 3, flights, src: 0, dst: 2, k: 0);

        Assert.Equal(500, price);
    }

    [Fact]
    public void FindCheapestPrice_DestinationUnreachableWithinStops_ReturnsNegativeOne()
    {
        int[][] flights = [[0, 1, 100]];

        var price = FindCheapestPrice(n: 3, flights, src: 0, dst: 2, k: 5);

        Assert.Equal(-1, price);
    }

    private static int FindCheapestPrice(int n, int[][] flights, int src, int dst, int k)
    {
        var maxEdges = k + 1;
        var states = new FlightState[n, maxEdges + 1];

        for (var city = 0; city < n; city++)
        {
            for (var layer = 0; layer <= maxEdges; layer++)
            {
                states[city, layer] = new FlightState(city, layer);
            }
        }

        foreach (var flight in flights)
        {
            var (from, to, price) = (flight[0], flight[1], flight[2]);

            for (var layer = 0; layer < maxEdges; layer++)
            {
                states[from, layer].Edges.Add((price, states[to, layer + 1]));
            }
        }

        var distances = ShortestPath.Dijkstra<
            FlightState, FlightStateTopology, ListEdges<FlightState, int>, int>(
            states[src, 0]);

        var best = int.MaxValue;

        for (var layer = 0; layer <= maxEdges; layer++)
        {
            if (distances.TryGetValue(states[dst, layer], out var price) && price < best)
            {
                best = price;
            }
        }

        return best == int.MaxValue ? -1 : best;
    }
}
