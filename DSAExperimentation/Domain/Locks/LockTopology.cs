using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Domain.Locks;

// A lock graph is a general graph, not a DAG: every wheel turn is reversible, so
// each edge appears in both directions and 2-cycles are everywhere.
internal readonly struct LockTopology : IGraphTopology<LockNode, ListChildren<LockNode>>
{
    public static ListChildren<LockNode> GetChildren(LockNode node) => new(node.Neighbors);
}
