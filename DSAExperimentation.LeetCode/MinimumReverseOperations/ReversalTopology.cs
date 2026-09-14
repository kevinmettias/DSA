using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumReverseOperations;

// Bare IGraphTopology: one reversal is undirected and the mirror relation is full of
// cycles, so nothing here may promise an engine there are none.
internal readonly struct ReversalTopology : IGraphTopology<PositionNode, ReversalChildren>
{
    public static ReversalChildren GetChildren(PositionNode node) => new(node);
}
