using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Modify Graph Edge Weights (LC 2699): both arms settle the same single -1 edge on
// a fixed-weight chain by repeated ShortestPath.Dijkstra calls over a mutable
// WeightedGraphNode graph, differing only in how a candidate weight is picked.
// LinearScanFromOne retries weight 1, 2, 3, ... - rerunning Dijkstra from scratch
// after each guess - until the shortest distance hits target exactly, an O(target)
// number of Dijkstra passes. TwoPassFormula instead reads the edge's two
// already-settled half-distances (source-to-u and v-to-destination) off two Dijkstra
// runs and computes the exact weight directly, needing only O(1) extra passes
// regardless of how large the stretch is.
[MemoryDiagnoser]
public class ModifyGraphEdgeWeightsBenchmarks
{
    private const int FixedEdgeWeight = 2;
    private const int ChainLength = 12;
    private const int Source = 0;
    private const int Destination = ChainLength - 1;

    // How far the single -1 edge (second-to-last hop) must stretch to reach target -
    // directly bounds LinearScanFromOne's Dijkstra-call count.
    [Params(50, 500)]
    public int StretchAmount;

    private int Target => FixedEdgeWeight * (ChainLength - 2) + StretchAmount;

    [Benchmark(Baseline = true)]
    public int LinearScanFromOne()
    {
        var (nodes, edgeRefs, negativeEdge) = BuildChain();
        var target = Target;

        for (var weight = 1; weight <= target; weight++)
        {
            SetEdgeWeight(nodes, edgeRefs[negativeEdge], weight);

            if (Distances(nodes, Source).TryGetValue(nodes[Destination], out var distance) && distance == target)
            {
                return weight;
            }
        }

        return -1;
    }

    [Benchmark]
    public int TwoPassFormula()
    {
        var (nodes, edgeRefs, negativeEdge) = BuildChain();
        var target = Target;
        var edgeRef = edgeRefs[negativeEdge];

        var fromSource = Distances(nodes, Source);
        var fromDestination = Distances(nodes, Destination);
        var candidate = target - fromSource[nodes[edgeRef.U]] - fromDestination[nodes[edgeRef.V]];

        SetEdgeWeight(nodes, edgeRef, candidate >= 1 ? candidate : 1);

        return Distances(nodes, Source).TryGetValue(nodes[Destination], out var distance) && distance == target
            ? candidate
            : -1;
    }

    private static Dictionary<WeightedGraphNode, int> Distances(WeightedGraphNode[] nodes, int source)
        => ShortestPath.Dijkstra<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(nodes[source]);

    // A straight chain 0-1-...-(ChainLength-1), every hop fixed at FixedEdgeWeight
    // except the last one, which starts at the floor weight 1 - the single edge
    // both benchmark arms grow toward Target.
    private static (WeightedGraphNode[] Nodes, (int U, int UIndex, int V, int VIndex)[] EdgeRefs, int NegativeEdge)
        BuildChain()
    {
        var nodes = new WeightedGraphNode[ChainLength];
        for (var i = 0; i < ChainLength; i++)
        {
            nodes[i] = new WeightedGraphNode(i);
        }

        var edgeCount = ChainLength - 1;
        var edgeRefs = new (int, int, int, int)[edgeCount];
        var negativeEdge = edgeCount - 1;

        for (var i = 0; i < edgeCount; i++)
        {
            var weight = i == negativeEdge ? 1 : FixedEdgeWeight;
            var uIndex = nodes[i].Edges.Count;
            nodes[i].Edges.Add((weight, nodes[i + 1]));
            var vIndex = nodes[i + 1].Edges.Count;
            nodes[i + 1].Edges.Add((weight, nodes[i]));
            edgeRefs[i] = (i, uIndex, i + 1, vIndex);
        }

        return (nodes, edgeRefs, negativeEdge);
    }

    private static void SetEdgeWeight(
        WeightedGraphNode[] nodes, (int U, int UIndex, int V, int VIndex) edgeRef, int weight)
    {
        nodes[edgeRef.U].Edges[edgeRef.UIndex] = (weight, nodes[edgeRef.V]);
        nodes[edgeRef.V].Edges[edgeRef.VIndex] = (weight, nodes[edgeRef.U]);
    }
}
