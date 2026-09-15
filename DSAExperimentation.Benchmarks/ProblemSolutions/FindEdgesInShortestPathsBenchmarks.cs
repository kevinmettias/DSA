using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindEdgesInShortestPathsSolution's, the same methods
// FindEdgesInShortestPathsTests proves correct. BruteForceDijkstra still takes
// LeetCode's own (n, edges) shape and builds its own BCL adjacency lists inside the
// measured call, deliberately without this repo's graph engine; ShortestPathDijkstra
// is handed the prepared EdgeGraph its hoisted overload takes, so graph construction
// is charged to [GlobalSetup] rather than to the search being measured.
[MemoryDiagnoser]
public class FindEdgesInShortestPathsBenchmarks
{
    private const int Seed = 3123;
    private const int ExtraEdgesPerNode = 2;

    private int[][] _edges = [];

    private EdgeGraph _graph = null!;
    [Params(50, 500)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _edges = FindEdgesInShortestPathsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, seed: Seed);
        _graph = EdgeGraph.Build(NodeCount, _edges);
    }

    [Benchmark(Baseline = true)]
    public bool[] BruteForceDijkstra() =>
        FindEdgesInShortestPathsSolution.AnswerByBruteForceDijkstra(NodeCount, _edges);

    [Benchmark]
    public bool[] ShortestPathDijkstra() =>
        FindEdgesInShortestPathsSolution.AnswerByShortestPathDijkstra(_graph);
}
