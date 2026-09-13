namespace DSAExperimentation.LeetCode.SortItemsByGroupsRespectingDependencies;

// Edges point prerequisite-group -> dependent-group, derived from any item edge
// whose two endpoints sit in different groups - mirrors ItemNode's own edge
// direction one level up, and answers LC 1203 alone.
internal sealed class GroupNode(int id)
{
    public int Id { get; } = id;

    public List<GroupNode> EnabledGroups { get; } = [];

    public override string ToString() => Id.ToString();
}
