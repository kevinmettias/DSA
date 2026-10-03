using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst;

public sealed partial class DepthFirstTraversalTests
{
    // TestTrees.NArySample's node count, every one of which a whole walk enters and exits.
    private const int NArySampleNodeCount = 7;

    // TestGraphs.CycleWithLeaf's node count: the cycle A -> B -> C -> A plus the leaf D.
    private const int CycleWithLeafNodeCount = 4;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Walk_Enter_FiresInPreOrder() =>
        Assert.Equal(
            new[] { "A", "B", "E", "F", "C", "D", "G" },
            Recorded(TestTrees.NArySample(), log => new RecordingEnterHooks(log)).Select(v => v.Name));

    [Fact]
    public void Walk_Exit_FiresInPostOrder() =>
        Assert.Equal(
            new[] { "E", "F", "B", "C", "G", "D", "A" },
            Recorded(TestTrees.NArySample(), log => new RecordingExitHooks(log)).Select(v => v.Name));

    [Fact]
    public void Walk_EnterAndExit_BothFireInOnePassAtCorrectOrders()
    {
        var entered = new List<(string Name, int Depth)>();
        var exited = new List<(string Name, int Depth)>();

        DepthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            RecordingEnterExitHooks>(TestTrees.NArySample(), new RecordingEnterExitHooks(entered, exited));

        Assert.Equal(new[] { "A", "B", "E", "F", "C", "D", "G" }, entered.Select(v => v.Name));
        Assert.Equal(new[] { "E", "F", "B", "C", "G", "D", "A" }, exited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_NullRoot_NoVisits() => Assert.Empty(Recorded(null, log => new RecordingEnterHooks(log)));

    // A hook held by value comes back with what the walk did to it, not as it went in.
    [Fact]
    public void Walk_ReturnsTheHookValueTheWalkFinishedWith()
    {
        var hooks = DepthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            CountingEnterExitHooks>(TestTrees.NArySample(), new CountingEnterExitHooks());

        Assert.Equal(NArySampleNodeCount, hooks.Entered);
        Assert.Equal(NArySampleNodeCount, hooks.Exited);
    }

    [Fact]
    public void Walk_NullRoot_ReturnsTheHookUnchanged()
    {
        var hooks = DepthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            CountingEnterExitHooks>(null, new CountingEnterExitHooks());

        Assert.Equal(0, hooks.Entered);
    }

    [Fact]
    public void Walk_ChildOrderIsOrthogonalToVisitTiming()
    {
        // Same pre-order timing, but every sibling group is walked back-to-front -
        // child order and visit-timing are independent knobs.
        var entered = new List<(string Name, int Depth)>();

        DepthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            RecordingEnterHooks>(TestTrees.NArySample(), new RecordingEnterHooks(entered));

        Assert.Equal(new[] { "A", "D", "G", "C", "B", "F", "E" }, entered.Select(v => v.Name));
    }

    // A -> [B, D], B -> C, C -> A (TestGraphs.CycleWithLeaf)
    [Fact]
    public void WalkGraph_VisitsEachNodeOnceDespiteCycle()
    {
        var entered = new List<(string Name, int Depth)>();
        var exited = new List<(string Name, int Depth)>();

        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            RecordingEnterExitHooks>(TestGraphs.CycleWithLeaf(), new RecordingEnterExitHooks(entered, exited));

        Assert.Equal(new[] { ("A", 0), ("B", 1), ("C", 2), ("D", 1) }, entered);
        Assert.Equal(new[] { ("C", 2), ("B", 1), ("D", 1), ("A", 0) }, exited);
    }

    [Fact]
    public void WalkGraph_NullRoot_NoVisits()
    {
        var entered = new List<(string Name, int Depth)>();

        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            RecordingEnterHooks>(null, new RecordingEnterHooks(entered));

        Assert.Empty(entered);
    }

    [Fact]
    public void WalkGraph_RootAlreadyInTheVisitedSet_NoVisits()
    {
        var root = TestGraphs.CycleWithLeaf();
        var entered = new List<(string Name, int Depth)>();

        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            RecordingEnterHooks>(root, new RecordingEnterHooks(entered), [root]);

        Assert.Empty(entered);
    }

    [Fact]
    public void WalkGraph_VisitedSet_CarriesAcrossCalls()
    {
        // X points into the cycle an earlier call already walked; the shared set is what
        // keeps the second walk to X alone.
        var cycle = TestGraphs.CycleWithLeaf();
        HashSet<TestNode> visited = [];
        EnteredWalkingGraphFrom(cycle, visited);

        var second = EnteredWalkingGraphFrom(new TestNode("X") { Children = { cycle } }, visited);

        Assert.Equal(new[] { ("X", 0) }, second);
    }

    [Fact]
    public void WalkGraph_ReturnsTheHookValueTheWalkFinishedWith()
    {
        var hooks = DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            CountingEnterExitHooks>(TestGraphs.CycleWithLeaf(), new CountingEnterExitHooks());

        Assert.Equal(CycleWithLeafNodeCount, hooks.Entered);
    }

    // Walks root with a hook built over a fresh log, and returns the log: a pre-order hook fills it
    // as nodes are entered, a post-order hook as they are exited.
    private static List<(string Name, int Depth)> Recorded<THooks>(
        TestNode? root, Func<List<(string Name, int Depth)>, THooks> hooksOver)
        where THooks : struct, IDepthFirstHooks<TestNode>
    {
        var log = new List<(string Name, int Depth)>();

        DepthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            THooks>(root, hooksOver(log));

        return log;
    }

    private static List<(string Name, int Depth)> EnteredWalkingGraphFrom(TestNode root, HashSet<TestNode> visited)
    {
        var entered = new List<(string Name, int Depth)>();

        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            RecordingEnterHooks>(root, new RecordingEnterHooks(entered), visited);

        return entered;
    }

    // HooksStep is private to DepthFirstTraversal: it is how a hook rides the depth-first
    // reduce as its state, so the traversal's own entry points are the way in. The (root)
    // overloads start from HooksStep's Seed, the hook's default value.
    public sealed partial class HooksStepTests
    {
        [Fact]
        public void Seed_StartsAWalkWithNoHookOfItsOwnFromTheDefaultHook()
        {
            var tree = DepthFirstTraversal.Walk<
                TestNode, TestTopology, ListChildren<TestNode>,
                CountingEnterExitHooks>(TestTrees.NArySample());
            var graph = DepthFirstTraversal.WalkGraph<
                TestNode, TestTopology, ListChildren<TestNode>,
                CountingEnterExitHooks>(TestGraphs.CycleWithLeaf());

            // Every node is entered and exited exactly once, so both counts start from zero.
            Assert.Equal((NArySampleNodeCount, NArySampleNodeCount), (tree.Entered, tree.Exited));
            Assert.Equal((CycleWithLeafNodeCount, CycleWithLeafNodeCount), (graph.Entered, graph.Exited));
        }

        [Fact]
        public void Enter_RunsTheHooksEnterAndPassesTheHookOn() =>
            Assert.Equal(NArySampleNodeCount, WalkCounting().Entered);

        [Fact]
        public void Exit_RunsTheHooksExitAndPassesTheHookOn() =>
            Assert.Equal(NArySampleNodeCount, WalkCounting().Exited);

        private static CountingEnterExitHooks WalkCounting()
            => DepthFirstTraversal.Walk<
                TestNode, TestTopology, ListChildren<TestNode>,
                CountingEnterExitHooks>(TestTrees.NArySample(), new CountingEnterExitHooks());
    }
}
