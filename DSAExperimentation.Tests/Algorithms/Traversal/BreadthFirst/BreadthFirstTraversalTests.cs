using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst;

public sealed partial class BreadthFirstTraversalTests
{
    // TestTrees.NArySample's node count, every one of which a whole walk visits.
    private const int NArySampleNodeCount = 7;

    // TestGraphs.CycleWithLeaf's node count: the cycle A -> B -> C -> A plus the leaf D.
    private const int CycleWithLeafNodeCount = 4;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Walk_VisitsInBreadthFirstOrderWithDepth()
    {
        var visited = new List<(string Name, int Depth)>();

        BreadthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks>(TestTrees.NArySample(), new RecordingVisitHooks(visited));

        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("C", 1), ("D", 1), ("E", 2), ("F", 2), ("G", 2) },
            visited);
    }

    [Fact]
    public void Walk_NullRoot_NoVisits()
    {
        var visited = new List<(string Name, int Depth)>();

        BreadthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks>(null, new RecordingVisitHooks(visited));

        Assert.Empty(visited);
    }

    // A hook held by value comes back with what the walk did to it, not as it went in.
    [Fact]
    public void Walk_ReturnsTheHookValueTheWalkFinishedWith()
    {
        var hooks = BreadthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            CountingVisitHooks>(TestTrees.NArySample(), new CountingVisitHooks());

        Assert.Equal(NArySampleNodeCount, hooks.Visited);
    }

    // A -> [B, D], B -> C, C -> A (TestGraphs.CycleWithLeaf)
    [Fact]
    public void WalkGraph_VisitsEachNodeOnceDespiteCycle()
    {
        // C's edge back to A is dropped by the guard; D, on A's level-one frontier, is
        // visited before C on level two.
        var visited = new List<(string Name, int Depth)>();

        BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks>(TestGraphs.CycleWithLeaf(), new RecordingVisitHooks(visited));

        Assert.Equal(new[] { ("A", 0), ("B", 1), ("D", 1), ("C", 2) }, visited);
    }

    [Fact]
    public void WalkGraph_NullRoot_NoVisits()
    {
        var visited = new List<(string Name, int Depth)>();

        BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks>(null, new RecordingVisitHooks(visited));

        Assert.Empty(visited);
    }

    [Fact]
    public void WalkGraph_RootAlreadyInTheVisitedSet_NoVisits()
    {
        var root = TestGraphs.CycleWithLeaf();
        var visited = new List<(string Name, int Depth)>();

        BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks>(root, new RecordingVisitHooks(visited), [root]);

        Assert.Empty(visited);
    }

    [Fact]
    public void WalkGraph_VisitedSet_CarriesAcrossCalls()
    {
        // X points into the cycle an earlier call already walked; the shared set is what
        // keeps the second walk to X alone.
        var cycle = TestGraphs.CycleWithLeaf();
        HashSet<TestNode> seen = [];
        VisitedWalkingGraphFrom(cycle, seen);

        var second = VisitedWalkingGraphFrom(new TestNode("X") { Children = { cycle } }, seen);

        Assert.Equal(new[] { ("X", 0) }, second);
    }

    [Fact]
    public void WalkGraph_ReturnsTheHookValueTheWalkFinishedWith()
    {
        var hooks = BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            CountingVisitHooks>(TestGraphs.CycleWithLeaf(), new CountingVisitHooks());

        Assert.Equal(CycleWithLeafNodeCount, hooks.Visited);
    }

    private static List<(string Name, int Depth)> VisitedWalkingGraphFrom(TestNode root, HashSet<TestNode> seen)
    {
        var visited = new List<(string Name, int Depth)>();

        BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks>(root, new RecordingVisitHooks(visited), seen);

        return visited;
    }

    // HooksStep is private to BreadthFirstTraversal: it is how a hook rides the
    // breadth-first reduce as its state, so the traversal's own entry points are the way
    // in. The (root) overloads start from HooksStep's Seed, the hook's default value.
    public sealed partial class HooksStepTests
    {
        [Fact]
        public void Seed_StartsAWalkWithNoHookOfItsOwnFromTheDefaultHook()
        {
            var tree = BreadthFirstTraversal.Walk<
                TestNode, TestTopology, ListChildren<TestNode>,
                NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
                CountingVisitHooks>(TestTrees.NArySample());
            var graph = BreadthFirstTraversal.WalkGraph<
                TestNode, TestTopology, ListChildren<TestNode>,
                NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
                CountingVisitHooks>(TestGraphs.CycleWithLeaf());

            Assert.Equal(NArySampleNodeCount, tree.Visited);
            Assert.Equal(CycleWithLeafNodeCount, graph.Visited);
        }

        [Fact]
        public void Enter_RunsTheHooksVisitAndPassesTheHookOn()
        {
            var hooks = BreadthFirstTraversal.Walk<
                TestNode, TestTopology, ListChildren<TestNode>,
                NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
                CountingVisitHooks>(TestTrees.NArySample(), new CountingVisitHooks());

            Assert.Equal(NArySampleNodeCount, hooks.Visited);
        }
    }
}
