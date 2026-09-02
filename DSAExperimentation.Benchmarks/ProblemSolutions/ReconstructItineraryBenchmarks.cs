using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reconstruct Itinerary (LC 332): both benchmarks run the identical Hierholzer walk
// (DFS that consumes one ticket per step, appending a stop only once its airport is
// a dead end, then reversing) - they differ only in how each departure airport picks
// its next lexicographically-smallest unused destination. LinearScanSelection scans
// a plain List<string> for the minimum and removes it, O(k) per step. HeapSelection
// uses this repo's own Heap<string, MinHeapOrder<string>>, O(log k) per push/pop.
[MemoryDiagnoser]
public class ReconstructItineraryBenchmarks
{
    private const int AirportCount = 26;

    private const string StartingAirport = "A";

    [Params(200, 2_000)]
    public int TicketCount;

    private string[][] _tickets = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var airports = Enumerable.Range(0, AirportCount).Select(i => ((char)('A' + i)).ToString()).ToArray();

        _tickets = Enumerable.Range(0, TicketCount)
            .Select(_ => new[] { airports[random.Next(airports.Length)], airports[random.Next(airports.Length)] })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public List<string> LinearScanSelection()
    {
        var graph = new Dictionary<string, List<string>>();

        foreach (var ticket in _tickets)
        {
            if (!graph.TryGetValue(ticket[0], out var destinations))
            {
                destinations = new List<string>();
                graph[ticket[0]] = destinations;
            }

            destinations.Add(ticket[1]);
        }

        var route = new List<string>();
        Visit(StartingAirport, graph, route);
        route.Reverse();
        return route;
    }

    private static string SelectAndRemoveSmallest(List<string> destinations)
    {
        var bestIndex = 0;

        for (var i = 1; i < destinations.Count; i++)
        {
            if (string.CompareOrdinal(destinations[i], destinations[bestIndex]) < 0)
            {
                bestIndex = i;
            }
        }

        var next = destinations[bestIndex];
        destinations.RemoveAt(bestIndex);

        return next;
    }

    private static void Visit(string airport, Dictionary<string, List<string>> graph, List<string> route)
    {
        if (graph.TryGetValue(airport, out var destinations))
        {
            while (destinations.Count > 0)
            {
                var next = SelectAndRemoveSmallest(destinations);
                Visit(next, graph, route);
            }
        }

        route.Add(airport);
    }

    private static HashMap<string, Heap<string, MinHeapOrder<string>>> BuildHeapGraph(string[][] tickets)
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

        return graph;
    }

    private static void Visit(string airport, HashMap<string, Heap<string, MinHeapOrder<string>>> graph, List<string> route)
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

    [Benchmark]
    public List<string> HeapSelection()
    {
        var graph = BuildHeapGraph(_tickets);
        var route = new List<string>();
        Visit(StartingAirport, graph, route);
        route.Reverse();
        return route;
    }
}
