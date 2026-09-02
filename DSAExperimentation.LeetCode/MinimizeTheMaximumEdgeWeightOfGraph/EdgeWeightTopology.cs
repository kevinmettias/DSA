using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimizeTheMaximumEdgeWeightOfGraph;

// Proof that "adjacency computed from runtime, per-instance context" needs no new
// abstraction here either: GetChildren stays a pure, static function of the node,
// exactly like GridTopology, because the node (EdgeWeightNode) carries the
// reference to its shared graph and the threshold currently in force.
internal readonly struct EdgeWeightTopology : IGraphTopology<EdgeWeightNode, EdgeWeightChildren>
{
    public static EdgeWeightChildren GetChildren(EdgeWeightNode node) => new(node);
}
