using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.DataStructures.Graph.Grids;

/// <summary>
/// The edge topology witness for <see cref="WeightedGridNode"/>, reading each node's weighted edges from its edge list.
/// </summary>
public readonly struct WeightedGridTopology
    : IEdgeTopology<WeightedGridNode, ListEdges<WeightedGridNode, int>, int>
{
    public static ListEdges<WeightedGridNode, int> GetEdges(WeightedGridNode node) => new(node.Edges);
}
