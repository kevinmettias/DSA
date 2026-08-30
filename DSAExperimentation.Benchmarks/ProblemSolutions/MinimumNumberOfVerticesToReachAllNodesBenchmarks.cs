using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Vertices to Reach All Nodes (LC 1557): the textbook O(V*E)
// nested scan (for every node, rescan every edge looking for a match) vs. one O(V+E)
// pass into this repo's own Set<int> marking which nodes have an incoming edge - the
// same "textbook nested loop vs. one repo-primitive pass" contrast TwoSumBenchmarks
// already established, here for in-degree instead of a target sum. Edges always run
// from a lower to a higher node id (a real DAG, not just acyclic by luck), so node 0
// is always a guaranteed source alongside however many other roots the random
// generation produces.
[MemoryDiagnoser]
public class MinimumNumberOfVerticesToReachAllNodesBenchmarks
{
    [Params(200, 5_000)]
    public int NodeCount;

    private (int From, int To)[] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1557);
        var edges = new List<(int From, int To)>();

        for (var to = 1; to < NodeCount; to++)
        {
            var edgeCount = random.Next(1, 3);
            for (var e = 0; e < edgeCount; e++)
            {
                edges.Add((random.Next(to), to));
            }
        }

        _edges = [.. edges];
    }

    [Benchmark(Baseline = true)]
    public int[] NestedScanForZeroInDegree()
    {
        var result = new List<int>();

        for (var node = 0; node < NodeCount; node++)
        {
            var hasIncoming = false;
            foreach (var edge in _edges)
            {
                if (edge.To == node)
                {
                    hasIncoming = true;
                    break;
                }
            }

            if (!hasIncoming)
            {
                result.Add(node);
            }
        }

        return [.. result];
    }

    [Benchmark]
    public int[] SetTrackedInDegree()
    {
        var hasIncomingEdge = new Set<int>();
        foreach (var edge in _edges)
        {
            hasIncomingEdge.TryAdd(edge.To);
        }

        var result = new List<int>();
        for (var node = 0; node < NodeCount; node++)
        {
            if (!hasIncomingEdge.Has(node))
            {
                result.Add(node);
            }
        }

        return [.. result];
    }
}
