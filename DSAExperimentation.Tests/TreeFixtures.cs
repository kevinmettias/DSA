using DSAExperimentation.Trees;

namespace DSAExperimentation.Tests;

public sealed class TestNode(string name)
{
    public string Name { get; } = name;

    public List<TestNode> Children { get; } = [];

    public TestNode? Left { get; set; }

    public TestNode? Right { get; set; }

    public override string ToString() => Name;
}

public readonly struct TestTopology : ITreeTopology<TestNode>
{
    public static IEnumerable<TestNode> GetChildren(TestNode node) => node.Children;
}

public readonly struct TestBinaryTopology : IBinaryTreeTopology<TestNode>
{
    public static IEnumerable<TestNode> GetChildren(TestNode node)
    {
        if (node.Left is not null) yield return node.Left;
        if (node.Right is not null) yield return node.Right;
    }

    public static TestNode? GetLeft(TestNode node) => node.Left;

    public static TestNode? GetRight(TestNode node) => node.Right;
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

    //        1
    //       / \
    //      2   3
    //     / \
    //    4   5
    public static TestNode BinarySample()
    {
        var four = new TestNode("4");
        var five = new TestNode("5");
        var two = new TestNode("2") { Left = four, Right = five };
        var three = new TestNode("3");
        var one = new TestNode("1") { Left = two, Right = three };
        return one;
    }
}
