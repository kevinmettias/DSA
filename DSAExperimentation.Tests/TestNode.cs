namespace DSAExperimentation.Tests;

public sealed class TestNode(string name)
{
    public string Name { get; } = name;

    public List<TestNode> Children { get; } = [];

    public override string ToString() => Name;
}
