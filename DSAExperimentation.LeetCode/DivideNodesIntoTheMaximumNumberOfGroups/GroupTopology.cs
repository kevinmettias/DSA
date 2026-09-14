using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.DivideNodesIntoTheMaximumNumberOfGroups;

// LC 2493's edges are undirected and may well form cycles - an odd one is
// precisely the answer -1 - so nothing here promises acyclicity, matching
// PossibleBipartition's PersonTopology.
internal readonly struct GroupTopology : IGraphTopology<GroupNode, ListChildren<GroupNode>>
{
    public static ListChildren<GroupNode> GetChildren(GroupNode node) => new(node.Neighbors);
}
