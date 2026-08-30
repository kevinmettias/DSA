using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Cheapest Flights Within K Stops (LC 787): the textbook unmemoized DFS over every
// path with at most K+1 edges (exponential in K) against this repo's
// ShortestPath.Dijkstra run over a layered (city, edgesUsed) state expansion of
// the same flight list - the K-stop bound lives in the graph's shape (an edge only
// ever moves a state from stop layer L to L+1), so Dijkstra itself needs no
// K-aware logic at all. K scales with CityCount so the naive side's exponential
// blowup is actually exercised rather than trivialized at the larger size.
[MemoryDiagnoser]
public class CheapestFlightsWithinKStopsBenchmarks
{
    [Params(15, 40)]
    public int CityCount;

    private int _maxEdges;
    private int _dst;
    private List<(int To, int Price)>[] _adjacency = null!;
    private FlightState[,] _states = null!;

    [GlobalSetup]
    public void Setup()
    {
        // "At most K stops" is "at most K+1 edges" - _maxEdges is that bound.
        _maxEdges = Math.Max(2, CityCount / 6) + 1;
        _dst = CityCount - 1;

        var random = new Random(7);
        var flights = new List<(int From, int To, int Price)>();

        // A back edge per node guarantees reachability from city 0, then a couple
        // of extra random edges per node for branching density.
        for (var i = 1; i < CityCount; i++)
        {
            flights.Add((random.Next(i), i, random.Next(1, 100)));
        }

        for (var i = 0; i < CityCount; i++)
        {
            for (var e = 0; e < 2; e++)
            {
                var target = random.Next(CityCount);

                if (target != i)
                {
                    flights.Add((i, target, random.Next(1, 100)));
                }
            }
        }

        _adjacency = BuildAdjacency(CityCount, flights);
        _states = BuildStates(CityCount, flights, _maxEdges + 1);
    }

    [Benchmark(Baseline = true)]
    public int NaiveDfs()
    {
        var best = int.MaxValue;
        Dfs(0, _maxEdges, 0, ref best);
        return best == int.MaxValue ? -1 : best;
    }

    [Benchmark]
    public int DijkstraOverStopLayeredGraph()
    {
        var distances = ShortestPath.Dijkstra<
            FlightState, FlightStateTopology, ListEdges<FlightState, int>, int>(_states[0, 0]);

        var best = int.MaxValue;

        for (var layer = 0; layer <= _maxEdges; layer++)
        {
            if (distances.TryGetValue(_states[_dst, layer], out var price) && price < best)
            {
                best = price;
            }
        }

        return best == int.MaxValue ? -1 : best;
    }

    private void Dfs(int city, int edgesLeft, int costSoFar, ref int best)
    {
        if (city == _dst && costSoFar < best)
        {
            best = costSoFar;
        }

        if (edgesLeft == 0)
        {
            return;
        }

        foreach (var (to, price) in _adjacency[city])
        {
            Dfs(to, edgesLeft - 1, costSoFar + price, ref best);
        }
    }

    private static List<(int To, int Price)>[] BuildAdjacency(
        int cityCount, List<(int From, int To, int Price)> flights)
    {
        var adjacency = Enumerable.Range(0, cityCount).Select(_ => new List<(int To, int Price)>()).ToArray();

        foreach (var (from, to, price) in flights)
        {
            adjacency[from].Add((to, price));
        }

        return adjacency;
    }

    private static FlightState[,] BuildStates(
        int cityCount, List<(int From, int To, int Price)> flights, int layerCount)
    {
        var states = new FlightState[cityCount, layerCount];

        for (var city = 0; city < cityCount; city++)
        {
            for (var layer = 0; layer < layerCount; layer++)
            {
                states[city, layer] = new FlightState(city, layer);
            }
        }

        foreach (var (from, to, price) in flights)
        {
            for (var layer = 0; layer < layerCount - 1; layer++)
            {
                states[from, layer].Edges.Add((price, states[to, layer + 1]));
            }
        }

        return states;
    }

    // See CheapestFlightsWithinKStopsTests.Fixtures for the full explanation -
    // repeated here rather than shared because TwoSumBenchmarks/
    // MedianOfTwoSortedArraysBenchmarks establish this project keeps its own copy
    // of the solution rather than depending on the Tests project.
    private sealed class FlightState(int city, int stops)
    {
        public int City { get; } = city;

        public int Stops { get; } = stops;

        public List<(int Price, FlightState Target)> Edges { get; } = [];
    }

    private readonly struct FlightStateTopology : IEdgeTopology<FlightState, ListEdges<FlightState, int>, int>
    {
        public static ListEdges<FlightState, int> GetEdges(FlightState node) => new(node.Edges);
    }
}
