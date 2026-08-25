using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

// Generic projection from any IEdgeTopology to plain IGraphTopology, via
// EdgeTargets - lets an edge-aware topology be handed to every existing
// IChildren-based algorithm (GraphReduce, TreeFold, ...) without hand-writing a
// wrapper per topology. This is the composition IEdgeTopology relies on instead of
// inheriting from IGraphTopology directly - GetEdges and GetChildren have
// different shapes, so this can't be a subtyping relationship, only a projection.
internal readonly struct EdgeTopologyAsGraphTopology<TNode, TTopology, TEdges, TEdgeData>
    : IGraphTopology<TNode, EdgeTargets<TNode, TEdgeData, TEdges>>
    where TNode : class
    where TTopology : struct, IEdgeTopology<TNode, TEdges, TEdgeData>
    where TEdges : struct, IEdges<TNode, TEdgeData>
{
    public static EdgeTargets<TNode, TEdgeData, TEdges> GetChildren(TNode node)
        => new(TTopology.GetEdges(node));
}
