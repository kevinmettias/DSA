using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumNumberOfVisitedCellsInAGrid;

// Every jump moves strictly right or strictly down, so the relation is in fact
// acyclic - but plain IGraphTopology is what Reduce.Graph's breadth-first order
// takes, and nothing here needs the stronger promise.
internal readonly struct JumpGridTopology : IGraphTopology<JumpGridNode, JumpGridChildren>
{
    public static JumpGridChildren GetChildren(JumpGridNode node) => new(node);
}
