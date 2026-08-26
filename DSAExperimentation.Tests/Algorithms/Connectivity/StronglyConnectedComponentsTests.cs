using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;
using StronglyConnectedComponentsOperations = DSAExperimentation.Algorithms.Connectivity.StronglyConnectedComponents;

namespace DSAExperimentation.Tests.Algorithms.Connectivity;

public sealed partial class StronglyConnectedComponentsTests
{
    // SCC membership, not discovery order, is the contract this algorithm promises - so
    // comparisons normalize both the node names within a component and the components
    // themselves to a stable order rather than asserting the algorithm's internal traversal
    // order.
    private static List<List<string>> Normalize(List<List<TestNode>> components)
    {
        var normalized = components
            .Select(component => component.Select(node => node.Name).OrderBy(name => name).ToList())
            .OrderBy(component => component[0])
            .ToList();

        return normalized;
    }

    [Fact]
    public void Tarjan_SimpleCycle_ReturnsOneComponent()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.Add(b);
        b.Children.Add(c);
        c.Children.Add(a);

        var components = StronglyConnectedComponentsOperations.Tarjan<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c]);
        var normalized = Normalize(components);

        Assert.Equal([new List<string> { "A", "B", "C" }], normalized);
    }

    [Fact]
    public void Tarjan_Dag_ReturnsOneSingletonComponentPerNode()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        a.Children.Add(b);
        b.Children.Add(c);

        var components = StronglyConnectedComponentsOperations.Tarjan<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c]);
        var normalized = Normalize(components);

        Assert.Equal(
            [new List<string> { "A" }, new List<string> { "B" }, new List<string> { "C" }],
            normalized);
    }

    // Two cycles joined by one one-way edge - the classic Tarjan boundary case: B -> C
    // must not merge the two cycles into a single component, since C can never reach back
    // to A or B.
    [Fact]
    public void Tarjan_TwoCyclesJoinedByOneWayEdge_ReturnsTwoComponents()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");
        a.Children.Add(b);
        b.Children.Add(a);
        b.Children.Add(c);
        c.Children.Add(d);
        d.Children.Add(c);

        var components = StronglyConnectedComponentsOperations.Tarjan<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c, d]);
        var normalized = Normalize(components);

        Assert.Equal(
            [new List<string> { "A", "B" }, new List<string> { "C", "D" }],
            normalized);
    }

    [Fact]
    public void Tarjan_SelfLoop_ReturnsSingletonComponent()
    {
        var a = new TestNode("A");
        a.Children.Add(a);

        var components = StronglyConnectedComponentsOperations.Tarjan<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a]);
        var normalized = Normalize(components);

        Assert.Equal([new List<string> { "A" }], normalized);
    }

    [Fact]
    public void Tarjan_DisconnectedGraph_ReturnsOneComponentPerPiece()
    {
        var a = new TestNode("A");
        var b = new TestNode("B");
        a.Children.Add(b);
        b.Children.Add(a);

        var c = new TestNode("C");

        var components = StronglyConnectedComponentsOperations.Tarjan<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            [a, b, c]);
        var normalized = Normalize(components);

        Assert.Equal(
            [new List<string> { "A", "B" }, new List<string> { "C" }],
            normalized);
    }

    [Fact]
    public void Tarjan_EmptyInput_ReturnsEmptyResult()
    {
        var components = StronglyConnectedComponentsOperations.Tarjan<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(
            []);

        Assert.Empty(components);
    }
}
