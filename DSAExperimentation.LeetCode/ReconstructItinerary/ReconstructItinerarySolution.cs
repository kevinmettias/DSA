using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.ReconstructItinerary;

// LeetCode 332. Reconstruct Itinerary: every itinerary starts at "JFK", and
// Hierholzer's algorithm (DFS that consumes exactly one ticket per step, appending a
// stop only once its airport is a dead end, then reversing) reconstructs a route that
// uses every ticket exactly once, preferring the lexicographically smallest
// destination at each branch. The two strategies differ only in how each airport
// selects its next unused destination.
internal static class ReconstructItinerarySolution
{
    // The textbook answer: BCL Dictionary<string, List<string>>, scanning for the
    // lexicographically smallest destination and removing it at each step.
    // Deliberately written without this repo's primitives - it is the arm
    // HeapSelection has to justify itself against.
    public static List<string> FindItineraryByLinearScan(string[][] tickets)
    {
        var graph = new Dictionary<string, List<string>>();

        foreach (var ticket in tickets)
        {
            if (!graph.TryGetValue(ticket[0], out var destinations))
            {
                destinations = [];
                graph[ticket[0]] = destinations;
            }

            destinations.Add(ticket[1]);
        }

        var route = new List<string>();
        VisitByLinearScan(StartAirport.Code, graph, route);
        route.Reverse();
        return route;
    }

    // This repo's own HashMap + Heap: each airport's unused destinations live in a
    // MinHeapOrder<string> Heap, so the lexicographically smallest is always the next
    // pop, O(log k) per push/pop instead of LinearScan's O(k) scan-and-remove.
    public static List<string> FindItineraryByHeapSelection(string[][] tickets)
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
        VisitByHeapSelection(StartAirport.Code, graph, route);
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

    private static void VisitByLinearScan(
        string airport, Dictionary<string, List<string>> graph, List<string> route)
    {
        if (graph.TryGetValue(airport, out var destinations))
        {
            while (destinations.Count > 0)
            {
                VisitByLinearScan(SelectAndRemoveSmallest(destinations), graph, route);
            }
        }

        route.Add(airport);
    }

    private static void VisitByHeapSelection(
        string airport, HashMap<string, Heap<string, MinHeapOrder<string>>> graph, List<string> route)
    {
        if (graph.TryGetValue(airport, out var destinations))
        {
            while (destinations.TryPop(out var next))
            {
                VisitByHeapSelection(next, graph, route);
            }
        }

        route.Add(airport);
    }
}
