using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking.Fixtures;

internal static class WalkGraphs
{
    //      A
    //     / \
    //    B   C
    //   / \
    //  D   E
    public static TestNode Tree()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.AddRange([b, c]);
        b.Children.AddRange([new TestNode("D"), new TestNode("E")]);
        return a;
    }

    // A -> B -> C -> A: nothing terminates a walk here except the guard.
    public static TestNode Cycle()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.Add(b);
        b.Children.Add(c);
        c.Children.Add(a);
        return a;
    }

    // A's two children both point at the same D - a shared descendant, reachable
    // twice without any cycle existing.
    public static (TestNode Root, TestNode Shared) DiamondShare()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");
        a.Children.AddRange([b, c]);
        b.Children.Add(d);
        c.Children.Add(d);
        return (a, d);
    }
}
