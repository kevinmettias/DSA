using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Reducing;
using DSAExperimentation.Tests.Graph.Engines.Fixtures;
using DSAExperimentation.Tests.Graph.Fixtures;

namespace DSAExperimentation.Tests.Graph.Engines.Reducing;

public sealed partial class ReduceTests
{
    private struct ClosedBeforeMarker;

    [Fact]
    public void Reduce_DepthFirst_CountsNodes()
    {
        var root = TestTrees.NArySample();

        var count = Reduce.Tree<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            CountNodesReduceAlgebra,
            int>(root);

        Assert.Equal(7, count);
    }

    [Fact]
    public void Reduce_BreadthFirst_CountsNodes()
    {
        var root = TestTrees.NArySample();

        var count = Reduce.Tree<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            BreadthFirstReduceOrder<TestNode>,
            CountNodesReduceAlgebra,
            int>(root);

        Assert.Equal(7, count);
    }

    [Fact]
    public void Reduce_NullRoot_ReturnsSeed()
    {
        var count = Reduce.Tree<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            CountNodesReduceAlgebra,
            int>(null);

        Assert.Equal(0, count);
    }

    [Fact]
    public void Reduce_TraversalOrderChangesTheResult()
    {
        // Unlike fold, reduce threads a single accumulator through a linearization
        // of the tree, so a non-commutative accumulate (string concatenation) gives
        // a genuinely different answer for a different traversal order.
        var root = TestTrees.NArySample();
        var preOrderPath = PathVia<DepthFirstReduceOrder<TestNode>>(root);
        var breadthFirstPath = PathVia<BreadthFirstReduceOrder<TestNode>>(root);

        Assert.Equal("ABEFCDG", preOrderPath);
        Assert.Equal("ABCDEFG", breadthFirstPath);
        Assert.NotEqual(preOrderPath, breadthFirstPath);
    }

    private static string PathVia<TOrderStrategy>(TestNode root)
        where TOrderStrategy : struct, IReduceOrderStrategy<TestNode>
        => Reduce.Tree<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            TOrderStrategy,
            PathReduceAlgebra,
            string>(root);

    [Fact]
    public void Reduce_EnterAndExit_CanExpressCrossSubtreeSequentialState()
    {
        // Something a bottom-up fold structurally cannot see: how many OTHER nodes
        // elsewhere in the tree have already finished by the time this node opens.
        // That's a global sequential fact, not a function of this node's own
        // subtree, so it needs a single accumulator threaded across both Enter and
        // Exit - exactly the capability a single-event-per-node reduce would lack.
        var root = TestTrees.NArySample();

        Reduce.Tree<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            ClosedBeforeOpenReduceAlgebra<ClosedBeforeMarker>,
            int>(root);

        Assert.Equal(
            new[] { ("A", 0), ("B", 0), ("E", 0), ("F", 1), ("C", 3), ("D", 4), ("G", 4) },
            ClosedBeforeOpenReduceAlgebra<ClosedBeforeMarker>.Log);
    }
}
