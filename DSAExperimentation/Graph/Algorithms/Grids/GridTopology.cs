using DSAExperimentation.Graph.Contracts.Topologies;

namespace DSAExperimentation.Graph.Algorithms.Grids;

// Proof that "adjacency computed from runtime, per-instance context" needs no new
// abstraction: GetChildren stays a pure, static function of the node, exactly like
// every other IGraphTopology, because the node (GridNode) carries the reference to
// its shared Grid - the same way a linked TestNode carries its own Children list.
internal readonly struct GridTopology : IGraphTopology<GridNode, GridChildren>
{
    public static GridChildren GetChildren(GridNode node) => new(node);
}
