using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.PathWithMaximumProbability;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs to
// run over ProbabilityNode - the same one-line shape RecoveryTopology and
// WeightedGridTopology already use for their own weighted nodes.
internal readonly struct ProbabilityTopology
    : IEdgeTopology<ProbabilityNode, ListEdges<ProbabilityNode, double>, double>
{
    public static ListEdges<ProbabilityNode, double> GetEdges(ProbabilityNode node) => new(node.Edges);
}
