using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using static DSAExperimentation.LeetCode.DesignGraphWithShortestPathCalculator.DesignGraphWithShortestPathCalculatorSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignGraphWithShortestPathCalculatorSolution's,
// the same classes DesignGraphWithShortestPathCalculatorTests proves correct. LC
// 2642's real cost is repeated shortestPath(node1, node2) queries against a graph
// that can grow via addEdge between calls, so nothing may be cached and every
// query runs a fresh single-source search - ArrayDijkstra is the textbook O(V^2)
// per-query form (plain distance array, linear-scan min extraction, no priority
// queue), HeapDijkstra is this repo's own ShortestPath.Dijkstra on a
// Collections.Heap frontier, O((V + E) log V) per query. Both graphs are built in
// [GlobalSetup] from the same RandomWeightedGraphs workload
// ShortestPathAlgorithmBenchmarks and CourseScheduleIVBenchmarks already share, so
// construction is charged to setup rather than to the queries being measured.
[MemoryDiagnoser]
public class DesignGraphWithShortestPathCalculatorBenchmarks
{
    private const int ExtraEdgesPerNode = 3;
    private const int RandomSeed = 2642; // LC problem number
    private const int QueryCount = 200;

    [Params(50, 200)]
    public int NodeCount;

    private ShortestPathGraphByArrayDijkstra _arrayGraph = null!;
    private ShortestPathGraphByHeapDijkstra _heapGraph = null!;
    private (int Node1, int Node2)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var edges = RandomWeightedGraphs.BuildEdges(NodeCount, ExtraEdgesPerNode, RandomSeed);
        _arrayGraph = new ShortestPathGraphByArrayDijkstra(NodeCount, edges);
        _heapGraph = new ShortestPathGraphByHeapDijkstra(NodeCount, edges);
        _queries = BuildQueries(NodeCount, QueryCount, RandomSeed);
    }

    [Benchmark(Baseline = true)]
    public long ArrayDijkstra() => TotalShortestPath(_arrayGraph);

    [Benchmark]
    public long HeapDijkstra() => TotalShortestPath(_heapGraph);

    private long TotalShortestPath(IShortestPathGraph graph)
    {
        var total = 0L;

        foreach (var (node1, node2) in _queries)
        {
            total += graph.ShortestPathBetween(node1, node2);
        }

        return total;
    }

    private static (int Node1, int Node2)[] BuildQueries(int nodeCount, int queryCount, int seed)
    {
        var random = new Random(seed);
        var queries = new (int Node1, int Node2)[queryCount];

        for (var i = 0; i < queryCount; i++)
        {
            queries[i] = (random.Next(nodeCount), random.Next(nodeCount));
        }

        return queries;
    }
}
