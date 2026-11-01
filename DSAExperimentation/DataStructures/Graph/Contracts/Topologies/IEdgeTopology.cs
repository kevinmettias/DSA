using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

/// <summary>
/// Says, for any node, which edges leave it and what data each carries, promising nothing about cycles.
/// </summary>
public interface IEdgeTopology<TNode, TEdges, TEdgeData>
    where TNode : class
    where TEdges : struct, IEdges<TNode, TEdgeData>
{
    /// <summary>Returns the edges leaving a node, in the topology's own order.</summary>
    /// <param name="node">The node whose outgoing edges to return.</param>
    /// <returns>A view of the node's edges.</returns>
    static abstract TEdges GetEdges(TNode node);
}
