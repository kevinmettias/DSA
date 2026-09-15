using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfVerticesToReachAllNodes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfVerticesToReachAllNodesSolution's, the
// same methods MinimumNumberOfVerticesToReachAllNodesTests proves correct - the
// textbook O(V*E) nested scan (for every node, rescan every edge looking for a
// match) against one O(V+E) marking pass into this repo's own Set<int>. Edge
// generation is charged to [GlobalSetup]; edges always run from a lower to a higher
// node id (a real DAG, not just acyclic by luck), so node 0 is always a guaranteed
// source alongside however many other roots the random generation produces.
[MemoryDiagnoser]
public class MinimumNumberOfVerticesToReachAllNodesBenchmarks
{
    private const int RandomSeed = 1557; // LC problem number
    private const int EdgeCountUpperBoundExclusive = 3;

    private int[][] _edges = [];

    [Params(200, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edges = new List<int[]>();

        for (var to = 1; to < NodeCount; to++)
        {
            var edgeCount = random.Next(1, EdgeCountUpperBoundExclusive);
            for (var e = 0; e < edgeCount; e++)
            {
                edges.Add([random.Next(to), to]);
            }
        }

        _edges = [.. edges];
    }

    [Benchmark(Baseline = true)]
    public List<int> NestedScanForZeroInDegree() =>
        MinimumNumberOfVerticesToReachAllNodesSolution.FindSmallestSetOfVerticesByNestedScan(NodeCount, _edges);

    [Benchmark]
    public List<int> SetTrackedInDegree() =>
        MinimumNumberOfVerticesToReachAllNodesSolution.FindSmallestSetOfVerticesByInDegreeSet(NodeCount, _edges);
}
