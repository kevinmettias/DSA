namespace DSAExperimentation.Tests.Fixtures;

internal static class TestTrees
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
