using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.SortItemsByGroupsRespectingDependencies;

internal readonly struct ItemTopology : IGraphTopology<ItemNode, ListChildren<ItemNode>>
{
    public static ListChildren<ItemNode> GetChildren(ItemNode node) => new(node.EnabledItems);
}
