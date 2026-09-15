using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ModifyGraphEdgeWeights;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ModifyGraphEdgeWeightsSolution's, the same methods
// ModifyGraphEdgeWeightsTests proves correct. The workload is a straight chain
// whose every hop is fixed except the last, so exactly one -1 edge has to absorb
// the whole stretch - the linear scan pays one Dijkstra pass per unit of
// StretchAmount, the half-distance formula two passes regardless.
//
// [GlobalSetup] hoists only the raw edge array: each strategy reassigns weights
// as it searches, so the graph itself has to be rebuilt per invocation and cannot
// be prepared once - which is also why neither strategy carries a prepared-input
// overload (ARCHITECTURE.md 17.4).
[MemoryDiagnoser]
public class ModifyGraphEdgeWeightsBenchmarks
{
    private const int FixedEdgeWeight = 2;
    private const int ChainLength = 12;
    private const int Source = 0;
    private const int Destination = ChainLength - 1;

    private int[][] _edges = [];

    private int _target;
    // How far the single -1 edge (the chain's last hop) must stretch to reach
    // target - directly bounds LinearScanFromOne's Dijkstra-call count.
    [Params(50, 500)]
    public int StretchAmount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _edges = BuildChainEdges();
        _target = (FixedEdgeWeight * (ChainLength - 2)) + StretchAmount;
    }

    // A straight chain 0-1-...-(ChainLength-1), every hop fixed at FixedEdgeWeight
    // except the last, which LeetCode's -1 marker leaves for the solution to pick.
    private static int[][] BuildChainEdges()
    {
        var edgeCount = ChainLength - 1;
        var edges = new int[edgeCount][];

        for (var i = 0; i < edgeCount; i++)
        {
            var isLastHop = i == edgeCount - 1;
            edges[i] = [i, i + 1, isLastHop ? AssignableEdgeWeights.Unassigned : FixedEdgeWeight];
        }

        return edges;
    }

    [Benchmark(Baseline = true)]
    public int[][] LinearScanFromOne() =>
        ModifyGraphEdgeWeightsSolution.ModifyEdgeWeightsByLinearWeightScan(
            ChainLength, _edges, (Source: Source, Destination: Destination, Target: _target));

    [Benchmark]
    public int[][] TwoPassFormula() =>
        ModifyGraphEdgeWeightsSolution.ModifyEdgeWeightsByHalfDistanceFormula(
            ChainLength, _edges, (Source: Source, Destination: Destination, Target: _target));
}
