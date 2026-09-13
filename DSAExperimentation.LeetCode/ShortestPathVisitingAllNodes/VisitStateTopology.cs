using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.ShortestPathVisitingAllNodes;

// A visit state's children are the states one input-graph edge away, already
// materialized on the node itself, so the topology is a straight read of
// Neighbors - the same ListChildren witness LockTopology and FlightStateTopology
// expose.
internal readonly struct VisitStateTopology : IGraphTopology<VisitStateNode, ListChildren<VisitStateNode>>
{
    public static ListChildren<VisitStateNode> GetChildren(VisitStateNode node) => new(node.Neighbors);
}
