using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumTimeToTransportAllIndividuals;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra
// needs to run over TransportState - the same one-line shape
// DigitStepTopology already uses for DigitOperationsToMakeTwoIntegersEqual's
// own weighted node.
internal readonly struct TransportTopology
    : IEdgeTopology<TransportState, ListEdges<TransportState, double>, double>
{
    public static ListEdges<TransportState, double> GetEdges(TransportState node) => new(node.Edges);
}
