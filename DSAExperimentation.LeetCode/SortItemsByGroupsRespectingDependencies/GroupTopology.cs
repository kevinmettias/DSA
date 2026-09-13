using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.SortItemsByGroupsRespectingDependencies;

internal readonly struct GroupTopology : IGraphTopology<GroupNode, ListChildren<GroupNode>>
{
    public static ListChildren<GroupNode> GetChildren(GroupNode node) => new(node.EnabledGroups);
}
