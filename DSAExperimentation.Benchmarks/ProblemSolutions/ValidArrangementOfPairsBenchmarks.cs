using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using PairStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Valid Arrangement of Pairs (LC 2097): the same Hierholzer's-algorithm shape
// ReconstructItineraryBenchmarks already proves (a DFS that consumes one edge per
// step, appending a node to the route only once it's a dead end, then reversing) -
// this problem doesn't need destinations picked in any particular order, only that
// every pair gets consumed exactly once, so the two benchmarks differ only in how
// each node's remaining outgoing pairs are stored and removed. DictionaryWithList
// keeps a plain Dictionary<int, List<int>> and always removes the LAST entry
// (List.RemoveAt(Count-1), itself O(1), so this baseline isn't handicapped by an
// unrelated O(k) shift - the comparison stays about container overhead, not a
// rigged removal position). RepoHashMapWithStack uses this repo's own
// HashMap<int, Stack<int>>, whose Stack composes DynamicArray the exact same way -
// same asymptotic shape either side, so the difference is purely each container's
// own hashing/indirection cost.
[MemoryDiagnoser]
public class ValidArrangementOfPairsBenchmarks
{
    private const int NodeCount = 64;
    private const int StartNode = 0;
    private const int RandomSeed = 2097;

    [Params(200, 2_000)]
    public int PairCount;

    private int[][] _pairs = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _pairs = new int[PairCount][];

        for (var i = 0; i < PairCount; i++)
        {
            _pairs[i] = [random.Next(0, NodeCount), random.Next(0, NodeCount)];
        }
    }

    [Benchmark(Baseline = true)]
    public List<int> DictionaryWithList()
    {
        var graph = new Dictionary<int, List<int>>();

        foreach (var pair in _pairs)
        {
            if (!graph.TryGetValue(pair[0], out var destinations))
            {
                destinations = new List<int>();
                graph[pair[0]] = destinations;
            }

            destinations.Add(pair[1]);
        }

        var route = new List<int>();
        VisitList(StartNode, graph, route);
        route.Reverse();
        return route;
    }

    private static void VisitList(int node, Dictionary<int, List<int>> graph, List<int> route)
    {
        if (graph.TryGetValue(node, out var destinations))
        {
            while (destinations.Count > 0)
            {
                var next = destinations[^1];
                destinations.RemoveAt(destinations.Count - 1);
                VisitList(next, graph, route);
            }
        }

        route.Add(node);
    }

    [Benchmark]
    public List<int> RepoHashMapWithStack()
    {
        var graph = new HashMap<int, PairStack>();

        foreach (var pair in _pairs)
        {
            if (!graph.TryGetValue(pair[0], out var destinations))
            {
                destinations = new PairStack();
                graph.Set(pair[0], destinations);
            }

            destinations.Push(pair[1]);
        }

        var route = new List<int>();
        VisitStack(StartNode, graph, route);
        route.Reverse();
        return route;
    }

    private static void VisitStack(int node, HashMap<int, PairStack> graph, List<int> route)
    {
        if (graph.TryGetValue(node, out var destinations))
        {
            while (destinations.TryPop(out var next))
            {
                VisitStack(next, graph, route);
            }
        }

        route.Add(node);
    }
}
