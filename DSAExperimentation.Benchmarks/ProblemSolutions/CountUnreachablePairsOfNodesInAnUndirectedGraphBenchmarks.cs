using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountUnreachablePairsOfNodesInAnUndirectedGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountUnreachablePairsOfNodesInAnUndirectedGraph-
// Solution's, the same methods the tests prove correct - a DFS flood fill over an
// adjacency list against this repo's own DisjointSet plus a HashMap size tally. The
// graph is built as several disjoint chains rather than one connected component, so
// both strategies have multiple real components to discover and size, and edge
// construction is charged to [GlobalSetup] rather than to the measured arms.
[MemoryDiagnoser]
public class CountUnreachablePairsOfNodesInAnUndirectedGraphBenchmarks
{
    private const int NodesPerComponent = 25;

    private int[][] _edges = [];

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var cappedComponentSize = Math.Min(NodesPerComponent, NodeCount);
        var segmentSize = Math.Max(2, cappedComponentSize);
        var edges = new List<int[]>();

        for (var node = 1; node < NodeCount; node++)
        {
            if (node % segmentSize != 0)
            {
                edges.Add([node - 1, node]);
            }
        }

        _edges = [.. edges];
    }

    [Benchmark(Baseline = true)]
    public long DepthFirstFloodFill() =>
        CountUnreachablePairsOfNodesInAnUndirectedGraphSolution.CountPairsByDepthFirstFloodFill(NodeCount, _edges);

    [Benchmark]
    public long DisjointSetUnionFind() =>
        CountUnreachablePairsOfNodesInAnUndirectedGraphSolution.CountPairsByDisjointSet(NodeCount, _edges);
}
