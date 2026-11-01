using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.DataStructures.Graph.Adjacency;

// The edge-aware counterpart of AdjacencyTopology, with the same graph-tier-only promise.
/// <summary>
/// The edge topology over <see cref="WeightedAdjacencyNode{TWeight}"/>, giving a node's edges in list order.
/// </summary>
public readonly struct WeightedAdjacencyTopology<TWeight>
    : IEdgeTopology<WeightedAdjacencyNode<TWeight>, ListEdges<WeightedAdjacencyNode<TWeight>, TWeight>, TWeight>
{
    public static ListEdges<WeightedAdjacencyNode<TWeight>, TWeight> GetEdges(WeightedAdjacencyNode<TWeight> node)
        => new(node.Edges);
}
