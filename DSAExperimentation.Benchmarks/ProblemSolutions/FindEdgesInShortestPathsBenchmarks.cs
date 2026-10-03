using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindEdgesInShortestPathsSolution's, the same methods
// FindEdgesInShortestPathsSolutionTests proves correct. BruteForceDijkstra still takes
// LeetCode's own (n, edges) shape and builds its own BCL adjacency lists inside the
// measured call, deliberately without this repo's graph engine; ShortestPathDijkstra
// is handed the prepared EdgeGraph its hoisted overload takes, so graph construction
// is charged to [GlobalSetup] rather than to the search being measured. The shared
// workload can draw the same undirected edge twice, which LC 3123 rules out, so
// only the first draw of each pair is kept.
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
        var drawnEdges = FindEdgesInShortestPathsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, seed: Seed);
        _edges = drawnEdges.DistinctBy(UndirectedPair).ToArray();
        _graph = EdgeGraph.Build(NodeCount, _edges);
    }

    private static (int Low, int High) UndirectedPair(int[] edge) =>
        (Math.Min(edge[0], edge[1]), Math.Max(edge[0], edge[1]));

    [Benchmark(Baseline = true)]
    public bool[] BruteForceDijkstra() =>
        FindEdgesInShortestPathsSolution.AnswerByBruteForceDijkstra(NodeCount, _edges);

    [Benchmark]
    public bool[] ShortestPathDijkstra() =>
        FindEdgesInShortestPathsSolution.AnswerByShortestPathDijkstra(_graph);
}
