using DSAExperimentation.Graph.Algorithms.Connectivity;
using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Reducing;
using DSAExperimentation.Tests.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Connectivity;

public sealed partial class ConnectedComponentsTests
{
    [Fact]
    public void Count_IsOrderIndependent_AcrossSeveralDisjointComponents()
    {
        var scrambled = ScrambledDisjointComponents();
        // BreadthFirstReduceOrder here, DepthFirstReduceOrder in the test below -
        // deliberately different, since a component count can't depend on which
        // order it explores a component in.
        var count = ConnectedComponents.Count<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            BreadthFirstReduceOrder<TestNode>>(scrambled);

        Assert.Equal(3, count);
    }

    // Undirected (symmetric Children edges), three components: a path A-B-C, an isolated
    // D, and a pair E-F. Deliberately scrambled, not root-first, to prove the shared guard
    // makes the count correct regardless of which member of a component is enumerated first.
    private static TestNode[] ScrambledDisjointComponents()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");
        var e = new TestNode("E");
        var f = new TestNode("F");
        a.Children.Add(b);
        b.Children.Add(a);
        b.Children.Add(c);
        c.Children.Add(b);
        e.Children.Add(f);
        f.Children.Add(e);

        return [c, a, d, f, b, e];
    }

    [Fact]
    public void Count_EmptyUniverse_ReturnsZero()
    {
        var count = ConnectedComponents.Count<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>>([]);

        Assert.Equal(0, count);
    }
}
