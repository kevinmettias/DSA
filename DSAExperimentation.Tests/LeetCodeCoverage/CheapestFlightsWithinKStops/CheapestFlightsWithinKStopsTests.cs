using DSAExperimentation.LeetCode.CheapestFlightsWithinKStops;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheapestFlightsWithinKStops;

// Harness only. Both strategies - the unmemoized DFS baseline and the Dijkstra run
// over FlightStateGraph's stop-layered expansion - are
// CheapestFlightsWithinKStopsSolution's; this file just pins them to LeetCode's
// published examples plus the stop-bound edges they have to agree on: an
// unreachable destination, and src == dst costing nothing.
public sealed class CheapestFlightsWithinKStopsTests
{
    public static TheoryData<CheapestPriceExample> Examples =>
        new()
        {
            // LC example 1: two stops would cost 400, but K = 1 forces the 700 route.
            {
                new CheapestPriceExample(
                    N: 4,
                    Flights: [[0, 1, 100], [1, 2, 100], [2, 0, 100], [1, 3, 600], [2, 3, 200]],
                    Source: 0,
                    Destination: 3,
                    Stops: 1,
                    Expected: 700)
            },

            // The same network with the stop bound relaxed: 0 -> 1 -> 2 -> 3.
            {
                new CheapestPriceExample(
                    N: 4,
                    Flights: [[0, 1, 100], [1, 2, 100], [2, 0, 100], [1, 3, 600], [2, 3, 200]],
                    Source: 0,
                    Destination: 3,
                    Stops: 2,
                    Expected: 400)
            },

            // LC example 2: one stop allowed, so the two-hop route beats the direct flight.
            {
                new CheapestPriceExample(
                    N: 3,
                    Flights: [[0, 1, 100], [1, 2, 100], [0, 2, 500]],
                    Source: 0,
                    Destination: 2,
                    Stops: 1,
                    Expected: 200)
            },

            // LC example 3: no stops allowed, so only the direct flight qualifies.
            {
                new CheapestPriceExample(
                    N: 3,
                    Flights: [[0, 1, 100], [1, 2, 100], [0, 2, 500]],
                    Source: 0,
                    Destination: 2,
                    Stops: 0,
                    Expected: 500)
            },

            // Destination has no inbound flight at all, however many stops are allowed.
            {
                new CheapestPriceExample(
                    N: 3,
                    Flights: [[0, 1, 100]],
                    Source: 0,
                    Destination: 2,
                    Stops: 5,
                    Expected: -1)
            },

            // Already there: no flight needed.
            {
                new CheapestPriceExample(
                    N: 3,
                    Flights: [[0, 1, 100], [1, 2, 100]],
                    Source: 1,
                    Destination: 1,
                    Stops: 0,
                    Expected: 0)
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCheapestPriceByNaiveDfs_LeetCodeExamples_ReturnsCheapestPriceWithinStopBound(
        CheapestPriceExample example)
    {
        var price = CheapestFlightsWithinKStopsSolution.FindCheapestPriceByNaiveDfs(
            example.N,
            example.Flights,
            (Source: example.Source, Destination: example.Destination),
            example.Stops);

        Assert.Equal(example.Expected, price);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCheapestPriceByDijkstraOverStopLayers_LeetCodeExamples_ReturnsCheapestPriceWithinStopBound(
        CheapestPriceExample example)
    {
        var price = CheapestFlightsWithinKStopsSolution.FindCheapestPriceByDijkstraOverStopLayers(
            example.N,
            example.Flights,
            (Source: example.Source, Destination: example.Destination),
            example.Stops);

        Assert.Equal(example.Expected, price);
    }

    // One LeetCode example: the flight network, the endpoints to price, the stop bound, and
    // the cheapest price it allows (-1 when unreachable). Four of the six members are `int`
    // and their order is what the problem fixes, so the row names every position rather than
    // leaving six interchangeable arguments.
    public readonly record struct CheapestPriceExample(
        int N, int[][] Flights, int Source, int Destination, int Stops, int Expected);
}
