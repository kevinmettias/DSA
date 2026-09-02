namespace DSAExperimentation.Tests.LeetCodeCoverage.DivideNodesIntoTheMaximumNumberOfGroups.Fixtures;

internal sealed class GroupNode(int id)
{
    public int Id { get; } = id;

    public List<GroupNode> Neighbors { get; } = [];

    public override string ToString() => Id.ToString();
}
