namespace DSAExperimentation.Tests.Graph.Fixtures;

internal sealed class TestNode(string name)
{
    public string Name { get; } = name;

    public List<TestNode> Children { get; } = [];

    public override string ToString() => Name;
}
