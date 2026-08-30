namespace DSAExperimentation.Tests.LeetCodeCoverage.SortItemsByGroupsRespectingDependencies.Fixtures;

// Edges point prerequisite-group -> dependent-group, derived from any item edge
// whose two endpoints sit in different groups - mirrors ItemNode's own edge
// direction one level up.
internal sealed class GroupNode(int id)
{
    public int Id { get; } = id;

    public List<GroupNode> EnabledGroups { get; } = [];
}
