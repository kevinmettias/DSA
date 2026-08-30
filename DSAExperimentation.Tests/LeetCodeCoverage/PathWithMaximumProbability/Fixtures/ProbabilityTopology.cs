using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathWithMaximumProbability.Fixtures;

internal readonly struct ProbabilityTopology
    : IEdgeTopology<ProbabilityNode, ListEdges<ProbabilityNode, double>, double>
{
    public static ListEdges<ProbabilityNode, double> GetEdges(ProbabilityNode node) => new(node.Edges);
}
