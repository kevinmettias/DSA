using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ReachableNodesInSubdividedGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReachableNodesInSubdividedGraphSolution's, the same
// methods ReachableNodesInSubdividedGraphTests proves correct. MaterializedBfs still
// takes LeetCode's own (edges, maxMoves, n) shape and expands every subdivision node
// into BCL adjacency lists inside the measured call, deliberately without this repo's
// graph engine; Dijkstra is handed the prepared SubdividedGraph its hoisted overload
// takes, so building the weighted original graph is charged to [GlobalSetup] rather
// than to the search being measured.
[MemoryDiagnoser]
public class ReachableNodesInSubdividedGraphBenchmarks
{
    // LC problem number, used as the deterministic seed for graph generation.
    private const int RandomSeed = 882;

    private const int ExtraEdgesPerNode = 2;

    // Move budget scales with node count so larger graphs stay proportionally explorable.
    private const int MovesPerNodeBudget = 25;

    private int[][] _edges = [];

    private SubdividedGraph _graph = null!;
    private int _maxMoves;
    [Params(30, 150)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _edges = ReachableNodesInSubdividedGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, seed: RandomSeed);
        _graph = SubdividedGraph.Build(NodeCount, _edges);
        _maxMoves = NodeCount * MovesPerNodeBudget;
    }

    [Benchmark(Baseline = true)]
    public int MaterializedBfs() =>
        ReachableNodesInSubdividedGraphSolution.CountReachableNodesByMaterializedBfs(_edges, _maxMoves, NodeCount);

    [Benchmark]
    public int Dijkstra() =>
        ReachableNodesInSubdividedGraphSolution.CountReachableNodesByDijkstra(_graph, _maxMoves);
}
