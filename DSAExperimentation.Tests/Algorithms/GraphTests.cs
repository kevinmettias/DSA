using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.Algorithms.Reducing.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Graph.Engines;

public sealed partial class GraphTests
{
    private struct GraphDfsMarker;

    // A -> B -> C -> A (a cycle), plus A -> D (a leaf). TestTopology never promised
    // acyclicity beyond its name - nothing stops it being pointed at genuinely
    // cyclic data and handed to the graph-safe algorithms instead of the tree-only
    // ones.
    private static TestNode CyclicSample()
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

    [Fact]
    public void GraphReduce_DepthFirst_VisitsEachNodeOnceDespiteCycle()
    {
        var root = CyclicSample();

        var count = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            CountNodesReduceAlgebra, int>(root);

        Assert.Equal(4, count);
    }

    [Fact]
    public void GraphReduce_BreadthFirst_VisitsEachNodeOnceDespiteCycle()
    {
        var root = CyclicSample();

        var count = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            BreadthFirstReduceOrder<TestNode>,
            CountNodesReduceAlgebra, int>(root);

        Assert.Equal(4, count);
    }

    [Fact]
    public void DepthFirstTraversal_WalkGraph_VisitsEachNodeOnceDespiteCycle()
    {
        var root = CyclicSample();

        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingEnterHooks<GraphDfsMarker>>(root);

        Assert.Equal(4, RecordingEnterHooks<GraphDfsMarker>.Entered.Count);
    }

    [Fact]
    public void CheckedFold_TrueCycle_ThrowsInsteadOfHanging()
    {
        var root = CyclicSample();

        Assert.Throws<InvalidOperationException>(() =>
            CheckedFold.Fold<
                TestNode, TestTopology, ListChildren<TestNode>,
                NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
                CountNodesFoldAlgebra, int>(root));
    }
}
