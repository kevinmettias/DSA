using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.Fixtures;

internal readonly struct WeightedGraphTopology
    : IEdgeTopology<WeightedGraphNode, ListEdges<WeightedGraphNode, int>, int>
{
    public static ListEdges<WeightedGraphNode, int> GetEdges(WeightedGraphNode node) => new(node.Edges);
}
