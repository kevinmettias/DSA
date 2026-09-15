using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ShortestPathVisitingAllNodes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestPathVisitingAllNodesSolution's, the same
// methods ShortestPathVisitingAllNodesTests proves correct. The textbook arm
// walks the raw adjacency with a BCL Queue and one shared frontier seeded from
// every node at once; the composed arm is handed a prepared VisitStateGraph by
// its hoisted overload, so materializing the (node, mask) state space is charged
// to [GlobalSetup] rather than to the search being measured. The composed side
// is expected to do strictly more total work - it recomputes a full distance map
// per start instead of sharing one frontier - so this is also an honest look at
// that cost, the same spirit ShortestPathAlgorithmBenchmarks' FloydWarshall
// entry already documents.
[MemoryDiagnoser]
public class ShortestPathVisitingAllNodesBenchmarks
{
    private const int RandomSeed = 847;

    private int[][] _graph = [];

    private VisitStateGraph _stateGraph = null!;
    [Params(8, 11)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _graph = ShortestPathGraphs.BuildRandomConnectedGraph(NodeCount, seed: RandomSeed);
        _stateGraph = VisitStateGraph.Build(_graph);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() =>
        ShortestPathVisitingAllNodesSolution.ShortestPathLengthByMutationQueue(_graph);

    [Benchmark]
    public int ReduceGraphBfs() =>
        ShortestPathVisitingAllNodesSolution.ShortestPathLengthByReduceGraph(_stateGraph);
}
