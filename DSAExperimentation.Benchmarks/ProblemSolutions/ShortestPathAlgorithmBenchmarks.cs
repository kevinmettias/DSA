using DSAExperimentation.DataStructures.Graph.Adjacency;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Solutions.NetworkDelayTime;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The "which shortest-path algorithm" choice a problem like Network Delay Time
// (LC 743) or Cheapest Flights Within K Stops (LC 787) actually forces: every arm is
// one of NetworkDelayTimeSolution's own strategies, the same methods
// NetworkDelayTimeSolutionTests proves correct. The baseline is the textbook Dijkstra
// over a BCL PriorityQueue, handed the graph as LC 743's own [from, to, minutes] list
// over nodes 1..NodeCount; this repo's three algorithms are called through their
// generic prepared-input overload. Either way the graph is built in [GlobalSetup]
// rather than charged to the search being measured. All of them answer the same
// single-source-distances question
// on the same non-negative-weight graph, so this is a genuine algorithm swap, not an
// apples-to-oranges comparison. FloydWarshall computes every pair at once (a
// strictly harder question than the other two answer) - it is expected to lose here,
// and the point of including it is exactly that: showing when the all-pairs
// algorithm is the wrong tool for a single-source query, not just how fast it is in
// isolation.
//
// Deliberately at library level rather than under one LeetCode problem: LC 743 and LC 787 both
// force this same algorithm choice, so the suite is named for the choice, not for either problem.
// NodeCount stops at the 100 nodes both problems allow.
public class ShortestPathAlgorithmBenchmarks
{
    private const int ExtraEdgesPerNode = 3;
    private const int RandomSeed = 42;

    private List<WeightedAdjacencyNode<int>> _vertices = new();

    private WeightedAdjacencyNode<int> _source = null!;

    private int[][] _times = [];

    private int _sourceId;

    [Params(50, 100)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (vertices, source) = RandomWeightedGraphs.Build(NodeCount, extraEdgesPerNode: ExtraEdgesPerNode, seed: RandomSeed);
        _vertices = vertices;
        _source = source;

        // The fixture numbers nodes from 0; LC 743 numbers them from 1.
        _times = [.. RandomWeightedGraphs.BuildEdges(NodeCount, ExtraEdgesPerNode, RandomSeed).Select(OneBased)];
        _sourceId = source.Id + 1;
    }

    private static int[] OneBased(int[] edge) => [edge[0] + 1, edge[1] + 1, edge[2]];

    [Benchmark(Baseline = true)]
    public int PriorityQueueDijkstra()
        => NetworkDelayTimeSolution.MinutesToReachAllByPriorityQueue(_times, NodeCount, _sourceId);

    [Benchmark]
    public int Dijkstra()
        => NetworkDelayTimeSolution.MinutesToReachAllByDijkstra<
            WeightedAdjacencyNode<int>, WeightedAdjacencyTopology<int>>(_vertices, _source);

    [Benchmark]
    public int BellmanFord()
        => NetworkDelayTimeSolution.MinutesToReachAllByBellmanFord<
            WeightedAdjacencyNode<int>, WeightedAdjacencyTopology<int>>(_vertices, _source);

    [Benchmark]
    public int FloydWarshall()
        => NetworkDelayTimeSolution.MinutesToReachAllByFloydWarshall<
            WeightedAdjacencyNode<int>, WeightedAdjacencyTopology<int>>(_vertices, _source);
}
