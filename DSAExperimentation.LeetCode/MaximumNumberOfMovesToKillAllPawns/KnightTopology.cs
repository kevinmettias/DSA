using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MaximumNumberOfMovesToKillAllPawns;

// Pairs with KnightChildren the same way GridTopology pairs with GridChildren -
// a stateless witness so Reduce.Graph can walk knight-move adjacency exactly like
// any other IGraphTopology.
internal readonly struct KnightTopology : IGraphTopology<GridNode, KnightChildren>
{
    public static KnightChildren GetChildren(GridNode node) => new(node);
}
