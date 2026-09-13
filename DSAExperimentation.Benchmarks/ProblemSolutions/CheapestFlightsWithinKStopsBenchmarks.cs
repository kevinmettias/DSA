using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CheapestFlightsWithinKStops;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheapestFlightsWithinKStopsSolution's, the same
// methods CheapestFlightsWithinKStopsTests proves correct - the textbook
// unmemoized DFS over every path with at most K+1 edges (exponential in K) against
// this repo's ShortestPath.Dijkstra run over a layered (city, edgesUsed) state
// expansion of the same flight list. Each arm is handed the prepared input its
// hoisted overload takes - an adjacency FlightNetwork for the DFS, a built
// FlightStateGraph for Dijkstra - so construction is charged to [GlobalSetup]
// rather than to the search being measured. K scales with CityCount so the naive
// side's blowup is actually exercised at the larger size.
[MemoryDiagnoser]
public class CheapestFlightsWithinKStopsBenchmarks
{
    // Arbitrary fixed seed for reproducible benchmark input.
    private const int RandomSeed = 7;

    // Every itinerary starts at city 0 and ends at the last city, the farthest
    // point from the back-edge chain's root.
    private const int Source = 0;

    [Params(15, 40)]
    public int CityCount;

    private int _dst;
    private int _stops;
    private FlightNetwork _network = null!;
    private FlightStateGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        var flights = FlightWorkloads.BuildFlights(CityCount, seed: RandomSeed);

        _dst = CityCount - 1;
        _stops = FlightWorkloads.StopBound(CityCount);
        _network = FlightNetwork.Build(CityCount, flights);
        _graph = FlightStateGraph.Build(CityCount, flights, _stops);
    }

    [Benchmark(Baseline = true)]
    public int NaiveDfs() =>
        CheapestFlightsWithinKStopsSolution.FindCheapestPriceByNaiveDfs(_network, Source, _dst, _stops);

    [Benchmark]
    public int DijkstraOverStopLayeredGraph() =>
        CheapestFlightsWithinKStopsSolution.FindCheapestPriceByDijkstraOverStopLayers(_graph, Source, _dst);
}
