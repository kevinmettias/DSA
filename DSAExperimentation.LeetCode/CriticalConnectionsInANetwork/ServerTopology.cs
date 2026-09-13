using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.CriticalConnectionsInANetwork;

// LC 1192's network is undirected and cyclic by design - an edge on a cycle is
// precisely a connection that is *not* critical - so nothing here promises
// acyclicity, matching IsGraphBipartite's BipartiteTopology and
// Domain.Locks.LockTopology as general (non-tree) IGraphTopology witnesses.
internal readonly struct ServerTopology : IGraphTopology<ServerNode, ListChildren<ServerNode>>
{
    public static ListChildren<ServerNode> GetChildren(ServerNode node) => new(node.Neighbors);
}
