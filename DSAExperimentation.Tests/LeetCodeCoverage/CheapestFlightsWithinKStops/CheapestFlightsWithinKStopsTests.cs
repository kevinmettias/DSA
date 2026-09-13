using DSAExperimentation.LeetCode.CheapestFlightsWithinKStops;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheapestFlightsWithinKStops;

// Harness only. Both strategies - the unmemoized DFS baseline and the Dijkstra run
// over FlightStateGraph's stop-layered expansion - are
// CheapestFlightsWithinKStopsSolution's; this file just pins them to LeetCode's
// published examples plus the stop-bound edges they have to agree on: an
// unreachable destination, and src == dst costing nothing.
public sealed class CheapestFlightsWithinKStopsTests
{
    public static TheoryData<int, int[][], int, int, int, int> Examples =>
        new()
        {
            // LC example 1: two stops would cost 400, but K = 1 forces the 700 route.
            { 4, [[0, 1, 100], [1, 2, 100], [2, 0, 100], [1, 3, 600], [2, 3, 200]], 0, 3, 1, 700 },

            // The same network with the stop bound relaxed: 0 -> 1 -> 2 -> 3.
            { 4, [[0, 1, 100], [1, 2, 100], [2, 0, 100], [1, 3, 600], [2, 3, 200]], 0, 3, 2, 400 },

            // LC example 2: one stop allowed, so the two-hop route beats the direct flight.
            { 3, [[0, 1, 100], [1, 2, 100], [0, 2, 500]], 0, 2, 1, 200 },

            // LC example 3: no stops allowed, so only the direct flight qualifies.
            { 3, [[0, 1, 100], [1, 2, 100], [0, 2, 500]], 0, 2, 0, 500 },

            // Destination has no inbound flight at all, however many stops are allowed.
            { 3, [[0, 1, 100]], 0, 2, 5, -1 },

            // Already there: no flight needed.
            { 3, [[0, 1, 100], [1, 2, 100]], 1, 1, 0, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCheapestPriceByNaiveDfs_LeetCodeExamples_ReturnsCheapestPriceWithinStopBound(
        int n, int[][] flights, int src, int dst, int k, int expected) =>
        Assert.Equal(
            expected, CheapestFlightsWithinKStopsSolution.FindCheapestPriceByNaiveDfs(n, flights, src, dst, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCheapestPriceByDijkstraOverStopLayers_LeetCodeExamples_ReturnsCheapestPriceWithinStopBound(
        int n, int[][] flights, int src, int dst, int k, int expected) =>
        Assert.Equal(
            expected,
            CheapestFlightsWithinKStopsSolution.FindCheapestPriceByDijkstraOverStopLayers(n, flights, src, dst, k));
}
