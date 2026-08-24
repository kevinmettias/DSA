using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public sealed class TestNode(string name)
{
    public string Name { get; } = name;

    public List<TestNode> Children { get; } = [];

    public override string ToString() => Name;
}

public readonly struct TestTopology : ITreeTopology<TestNode, ListChildren<TestNode>>
{
    public static ListChildren<TestNode> GetChildren(TestNode node) => new(node.Children);
}

public static class TestTrees
{
    // A -> [B, C, D], B -> [E, F], D -> [G]
    public static TestNode NArySample()
    {
        var e = new TestNode("E");
        var f = new TestNode("F");
        var g = new TestNode("G");
        var b = new TestNode("B") { Children = { e, f } };
        var c = new TestNode("C");
        var d = new TestNode("D") { Children = { g } };
        var a = new TestNode("A") { Children = { b, c, d } };
        return a;
    }

    public static TestNode SingleNode() => new("A");
}
