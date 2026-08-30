namespace DSAExperimentation.Tests.LeetCodeCoverage.KthAncestorOfATreeNode.Fixtures;

internal sealed class TreeAncestorNode(int id)
{
    public int Id { get; } = id;

    public List<TreeAncestorNode> Children { get; } = [];

    public override string ToString() => Id.ToString();
}
