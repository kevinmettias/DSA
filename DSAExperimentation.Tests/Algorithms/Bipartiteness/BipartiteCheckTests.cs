using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;
using BipartiteCheckOperations = DSAExperimentation.Algorithms.Bipartiteness.BipartiteCheck;

namespace DSAExperimentation.Tests.Algorithms.Bipartiteness;

public sealed partial class BipartiteCheckTests
{
    private static void Connect(TestNode first, TestNode second)
    {
        first.Children.Add(second);
        second.Children.Add(first);
    }

    [Fact]
    public void IsBipartite_EvenCycle_ReturnsTrue()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");
        Connect(a, b);
        Connect(b, c);
        Connect(c, d);
        Connect(d, a);

        var result = BipartiteCheckOperations.IsBipartite<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c, d]);

        Assert.True(result);
    }

    [Fact]
    public void IsBipartite_OddCycle_ReturnsFalse()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        Connect(a, b);
        Connect(b, c);
        Connect(c, a);

        var result = BipartiteCheckOperations.IsBipartite<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c]);

        Assert.False(result);
    }

    [Fact]
    public void IsBipartite_OneBipartiteAndOneNonBipartiteComponent_ReturnsFalse()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        Connect(a, b);

        var x = new TestNode("X");
        var y = new TestNode("Y");
        var z = new TestNode("Z");
        Connect(x, y);
        Connect(y, z);
        Connect(z, x);

        var result = BipartiteCheckOperations.IsBipartite<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, x, y, z]);

        Assert.False(result);
    }

    [Fact]
    public void IsBipartite_SingleNodeWithNoEdges_ReturnsTrue()
    {
        var a = new TestNode("A");

        var result = BipartiteCheckOperations.IsBipartite<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a]);

        Assert.True(result);
    }

    [Fact]
    public void IsBipartite_EmptyInput_ReturnsTrue()
    {
        var result = BipartiteCheckOperations.IsBipartite<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            []);

        Assert.True(result);
    }
}
