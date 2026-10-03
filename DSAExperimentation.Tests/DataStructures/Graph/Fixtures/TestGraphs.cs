namespace DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

// Graph-shaped counterparts to TestTrees, for the graph-safe entry points (Reduce.Graph,
// the traversals' WalkGraph, CheckedFold) that exist precisely because TestTopology's
// tree promise can be broken by the data it is pointed at.
internal static class TestGraphs
{
    // A -> B -> C -> A (a cycle), plus A -> D (a leaf). The leaf sits after the cycle in
    // A's child order, so a walk that stopped at the cycle instead of skipping the
    // revisit would never reach it.
    public static TestNode CycleWithLeaf()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");
        a.Children.Add(b);
        a.Children.Add(d);
        b.Children.Add(c);
        c.Children.Add(a);
        return a;
    }

    // A -> [B, C], B -> D, C -> D: D is a shared descendant reached along two paths,
    // which is not a cycle.
    public static TestNode Diamond()
    {
        var d = new TestNode("D");
        var b = new TestNode("B") { Children = { d } };
        var c = new TestNode("C") { Children = { d } };
        return new TestNode("A") { Children = { b, c } };
    }
}
