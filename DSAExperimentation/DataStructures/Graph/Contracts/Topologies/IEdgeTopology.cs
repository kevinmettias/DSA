using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

internal interface IEdgeTopology<TNode, TEdges, TEdgeData>
    where TNode : class
    where TEdges : struct, IEdges<TNode, TEdgeData>
{
    static abstract TEdges GetEdges(TNode node);
}
