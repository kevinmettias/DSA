using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.DataStructures.Graph.Grids;

// Proof that "adjacency computed from runtime, per-instance context" needs no new
// abstraction: GetChildren stays a pure, static function of the node, exactly like
// every other IGraphTopology, because the node (GridNode) carries the reference to
// its shared Grid - the same way a linked TestNode carries its own Children list.
/// <summary>
/// The graph topology witness for <see cref="GridNode"/>, whose children are its open orthogonal neighbours.
/// </summary>
/// <typeparam name="TNodes">Which node each neighbour is handed as.</typeparam>
public readonly struct GridTopology<TNodes> : IGraphTopology<GridNode, GridChildren<TNodes>>
    where TNodes : struct, IGridNodeSource
{
    public static GridChildren<TNodes> GetChildren(GridNode node) => new(node);
}
