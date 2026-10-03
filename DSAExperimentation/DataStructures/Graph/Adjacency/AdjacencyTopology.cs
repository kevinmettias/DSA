using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.DataStructures.Graph.Adjacency;

// IGraphTopology and deliberately nothing stronger: an adjacency list can hold cycles and shared
// descendants, and only the caller knows whether a particular graph does not. A caller that can
// vouch for acyclicity writes its own one-line IDagTopology over AdjacencyNode, the same way
// RootedTreeTopology closes ITreeTopology over RootedTreeNode - the promise belongs to whoever can
// make it, not to the node.
internal readonly struct AdjacencyTopology : IGraphTopology<AdjacencyNode, ListChildren<AdjacencyNode>>
{
    public static ListChildren<AdjacencyNode> GetChildren(AdjacencyNode node) => new(node.Neighbors);
}
