using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.IsGraphBipartite;

// LC 785's graph is undirected and may be cyclic - an odd cycle is precisely the
// answer "false" - so nothing here promises acyclicity, matching
// Domain.Locks.LockTopology's own general (non-tree) IGraphTopology witness.
internal readonly struct BipartiteTopology : IGraphTopology<BipartiteNode, ListChildren<BipartiteNode>>
{
    public static ListChildren<BipartiteNode> GetChildren(BipartiteNode node) => new(node.Neighbors);
}
