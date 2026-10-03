using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.Algorithms.Reducing.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing;

public sealed partial class ReduceTests
{
    private struct ClosedBeforeMarker;

    [Fact]
    public void Tree_DepthFirst_CountsNodes()
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
    public void Tree_BreadthFirst_CountsNodes()
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
    public void Tree_NullRoot_ReturnsSeed()
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
    public void Tree_TraversalOrderChangesTheResult()
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
    public void Tree_EnterAndExit_CanExpressCrossSubtreeSequentialState()
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

    [Fact]
    public void Graph_DepthFirst_VisitsEachNodeOnceDespiteCycle()
    {
        var root = TestGraphs.CycleWithLeaf();

        var count = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            CountNodesReduceAlgebra, int>(root);

        Assert.Equal(4, count);
    }

    [Fact]
    public void Graph_BreadthFirst_VisitsEachNodeOnceDespiteCycle()
    {
        var root = TestGraphs.CycleWithLeaf();

        var count = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            BreadthFirstReduceOrder<TestNode>,
            CountNodesReduceAlgebra, int>(root);

        Assert.Equal(4, count);
    }

    [Fact]
    public void Graph_DepthFirst_SkipsTheEdgeBackToTheRootAndKeepsWalking()
    {
        // C's edge back to A is the only thing the guard drops: C still closes, and the
        // walk carries on into D, A's second child.
        var root = TestGraphs.CycleWithLeaf();

        var walk = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            NestingReduceAlgebra, string>(root);

        Assert.Equal("*(A0(B1(C2))(D1))", walk);
    }

    [Fact]
    public void Graph_NullRoot_ReturnsSeed()
    {
        var walk = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            NestingReduceAlgebra, string>(null);

        Assert.Equal(NestingReduceAlgebra.Seed, walk);
    }

    [Fact]
    public void Graph_RootAlreadyInTheVisitedSet_ReturnsSeedWithoutWalking()
    {
        var root = TestGraphs.CycleWithLeaf();
        HashSet<TestNode> visited = [root];

        var walk = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            NestingReduceAlgebra, string>(root, visited);

        Assert.Equal(NestingReduceAlgebra.Seed, walk);
        Assert.Single(visited);
    }

    [Fact]
    public void Graph_VisitedSet_CarriesAcrossCalls()
    {
        // X points into the cycle, which an earlier call has already walked: sharing
        // the set is what stops the second call re-walking it from X's edge.
        var cycle = TestGraphs.CycleWithLeaf();
        var x = new TestNode("X") { Children = { cycle } };
        HashSet<TestNode> visited = [];

        var first = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            NestingReduceAlgebra, string>(cycle, visited);
        var second = Reduce.Graph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            NestingReduceAlgebra, string>(x, visited);

        Assert.Equal("*(A0(B1(C2))(D1))", first);
        Assert.Equal("*(X0)", second);
        Assert.Equal(5, visited.Count);
    }
}
