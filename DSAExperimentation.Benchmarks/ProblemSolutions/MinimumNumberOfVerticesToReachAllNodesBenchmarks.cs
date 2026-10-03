using DSAExperimentation.LeetCode.MinimumNumberOfVerticesToReachAllNodes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfVerticesToReachAllNodesSolution's, the
// same methods MinimumNumberOfVerticesToReachAllNodesSolutionTests proves correct - the
// textbook O(V*E) nested scan (for every node, rescan every edge looking for a
// match) against one O(V+E) marking pass into this repo's own Set<int>. Edge
// generation is charged to [GlobalSetup]; edges always run from a lower to a higher
// node id (a real DAG, not just acyclic by luck), so node 0 is always a guaranteed
// source alongside however many other roots the random generation produces.
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
            AddEdgesInto(edges, to, random);
        }

        _edges = [.. edges];
    }

    // One or two edges into `to`, each from an earlier node. LC 1557's (from, to) pairs are
    // all distinct, so a second draw of a source already chosen adds nothing - and is still
    // drawn, so every later edge comes out of the seeded stream unchanged.
    private static void AddEdgesInto(List<int[]> edges, int to, Random random)
    {
        var edgeCount = random.Next(1, EdgeCountUpperBoundExclusive);
        var sources = new HashSet<int>();

        for (var e = 0; e < edgeCount; e++)
        {
            var from = random.Next(to);

            if (sources.Add(from))
            {
                edges.Add([from, to]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public List<int> NestedScanForZeroInDegree() =>
        MinimumNumberOfVerticesToReachAllNodesSolution.FindSmallestSetOfVerticesByNestedScan(NodeCount, _edges);

    [Benchmark]
    public List<int> SetTrackedInDegree() =>
        MinimumNumberOfVerticesToReachAllNodesSolution.FindSmallestSetOfVerticesByInDegreeSet(NodeCount, _edges);
}
