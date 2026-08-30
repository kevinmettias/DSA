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
    [Params(200, 2_000)]
    public int TicketCount;

    private string[][] _tickets = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var airports = Enumerable.Range(0, 26).Select(i => ((char)('A' + i)).ToString()).ToArray();

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
        Visit("A", graph, route);
        route.Reverse();
        return route;

        void Visit(string airport, Dictionary<string, List<string>> g, List<string> r)
        {
            if (g.TryGetValue(airport, out var destinations))
            {
                while (destinations.Count > 0)
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
                    Visit(next, g, r);
                }
            }

            r.Add(airport);
        }
    }

    [Benchmark]
    public List<string> HeapSelection()
    {
        var graph = new HashMap<string, Heap<string, MinHeapOrder<string>>>();

        foreach (var ticket in _tickets)
        {
            if (!graph.TryGetValue(ticket[0], out var destinations))
            {
                destinations = new Heap<string, MinHeapOrder<string>>();
                graph.Set(ticket[0], destinations);
            }

            destinations.Push(ticket[1]);
        }

        var route = new List<string>();
        Visit("A", graph, route);
        route.Reverse();
        return route;

        void Visit(string airport, HashMap<string, Heap<string, MinHeapOrder<string>>> g, List<string> r)
        {
            if (g.TryGetValue(airport, out var destinations))
            {
                while (destinations.TryPop(out var next))
                {
                    Visit(next, g, r);
                }
            }

            r.Add(airport);
        }
    }
}
