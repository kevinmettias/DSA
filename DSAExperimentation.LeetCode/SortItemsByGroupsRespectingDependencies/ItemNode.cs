namespace DSAExperimentation.LeetCode.SortItemsByGroupsRespectingDependencies;

// Edges point prerequisite -> dependent item, matching Kahn's algorithm's own
// in-degree bookkeeping - the same CourseNode shape (LC 210), with GroupId added so
// items can be bucketed by group once the item-level topological order is known.
// That GroupId is LC 1203's own two-level semantics and nothing else's, so this node
// lives beside the solution rather than in Domain/ or DataStructures/
// (ARCHITECTURE.md #17.3, #17.6).
internal sealed class ItemNode(int id, int groupId)
{
    public int Id { get; } = id;

    public int GroupId { get; } = groupId;

    public List<ItemNode> EnabledItems { get; } = [];

    public override string ToString() => Id.ToString();
}
