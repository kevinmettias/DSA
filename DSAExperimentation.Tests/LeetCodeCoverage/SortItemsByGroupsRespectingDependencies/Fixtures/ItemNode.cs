namespace DSAExperimentation.Tests.LeetCodeCoverage.SortItemsByGroupsRespectingDependencies.Fixtures;

// Edges point prerequisite -> dependent item, matching Kahn's algorithm's own
// in-degree bookkeeping - the same CourseNode shape (LC 210), with GroupId added so
// items can be bucketed by group once the item-level topological order is known.
internal sealed class ItemNode(int id, int groupId)
{
    public int Id { get; } = id;

    public int GroupId { get; } = groupId;

    public List<ItemNode> EnabledItems { get; } = [];
}
