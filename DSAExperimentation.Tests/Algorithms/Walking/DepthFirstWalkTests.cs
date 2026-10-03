using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;
using DSAExperimentation.Tests.Algorithms.Walking.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking;

// Harness only. DepthFirstWalk is the one depth-first engine, so every case names
// its graph in one vocabulary and the whole file reaches the engine through Build/
// WalkFrom/WalkReversed below rather than one builder per operation. A shape that
// repeats a node (a cycle, or two parents sharing a child) walks under the
// tracked guard, because an unguarded walk is valid only under ITreeTopology's
// unique-ancestry promise - the unguarded shared-descendant case below pins what
// breaking that promise costs. Names and depths are both expected per row because
// they are the same descend order read two ways, and the starting depth is a row
// value because a walk reports depth relative to where it was told to start.
public sealed partial class DepthFirstWalkTests
{
    public static TheoryData<WalkExample> Examples =>
        new()
        {
            { new WalkExample(GraphShape.Tree, 0, ["A", "B", "D", "E", "C"], [0, 1, 2, 2, 1]) },
            { new WalkExample(GraphShape.Single, 7, ["A"], [7]) },
            { new WalkExample(GraphShape.Cycle, 0, ["A", "B", "C"], [0, 1, 2]) },
            { new WalkExample(GraphShape.Diamond, 0, ["A", "B", "D", "C"], [0, 1, 2, 1]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void Walk_DescendsEachBranchFullyBeforeTheNext(WalkExample example)
    {
        var visited = Walk(example);

        Assert.Equal(example.Names, visited.Select(v => v.Name));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void Walk_ReportsDepthRelativeToTheStartingDepth(WalkExample example)
    {
        var visited = Walk(example);

        Assert.Equal(example.Depths, visited.Select(v => v.Depth));
    }

    [Fact]
    public void Walk_UnguardedOnASharedDescendant_VisitsItOncePerPath()
    {
        // Not a defect: UnguardedVisit is only valid under ITreeTopology's unique
        // ancestry promise, and this pins what breaking that promise costs.
        var (root, _) = WalkGraphs.DiamondShare();

        var visited = WalkFrom(root, 0, new UnguardedVisit<TestNode>());

        Assert.Equal(["A", "B", "D", "C", "D"], visited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_RespectsTheChildOrderWitness()
    {
        var root = Build(GraphShape.Tree);

        var visited = WalkReversed(root, 0, new UnguardedVisit<TestNode>());

        Assert.Equal(["A", "C", "B", "E", "D"], visited.Select(v => v.Name));
    }

    private static List<(string Name, int Depth)> Walk(WalkExample example)
    {
        var root = Build(example.Shape);
        var guard = GuardFor(root, example.Shape);

        return WalkFrom(root, example.StartDepth, guard);
    }

    private static List<(string Name, int Depth)> WalkFrom<TGuard>(TestNode root, int startDepth, TGuard guard)
        where TGuard : IVisitGuard<TestNode> =>
        DepthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TGuard, RecordingWalkStep, List<(string, int)>>(
            root, startDepth, [], guard);

    private static List<(string Name, int Depth)> WalkReversed<TGuard>(TestNode root, int startDepth, TGuard guard)
        where TGuard : IVisitGuard<TestNode> =>
        DepthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            TGuard, RecordingWalkStep, List<(string Name, int Depth)>>(
            root, startDepth, [], guard);

    // The graph-safe guard is seeded with the root, which is already visited by
    // definition; the tree-only shapes keep the unguarded visit they can rely on.
    private static IVisitGuard<TestNode> GuardFor(TestNode root, GraphShape shape)
    {
        if (IsTrackedGuardNeeded(shape))
        {
            return new TrackedVisitGuard<TestNode>([root]);
        }

        return new UnguardedVisit<TestNode>();
    }

    private static bool IsTrackedGuardNeeded(GraphShape shape) =>
        shape == GraphShape.Cycle || shape == GraphShape.Diamond;

    // WalkGraphs builds every shape but the single node; DiamondShare hands back the
    // root together with the node both of its children point at, and only the root
    // is needed to walk from.
    private static TestNode Build(GraphShape shape)
    {
        if (shape == GraphShape.Single)
        {
            return new TestNode("A");
        }

        if (shape == GraphShape.Tree)
        {
            return WalkGraphs.Tree();
        }

        if (shape == GraphShape.Cycle)
        {
            return WalkGraphs.Cycle();
        }

        return WalkGraphs.DiamondShare().Root;
    }

    public enum GraphShape
    {
        Single,
        Tree,
        Cycle,
        Diamond,
    }

    public readonly record struct WalkExample(GraphShape Shape, int StartDepth, string[] Names, int[] Depths);

    // HooksStep is private to DepthFirstWalk: it is how the void Walk overload runs a
    // pair of IDepthFirstHooks through the state-threading engine, so that overload is
    // the way in. Its Seed is not tested because nothing reads it - the void overload
    // starts the engine from default(Unit) directly.
    public sealed partial class HooksStepTests
    {
        private struct EnterMarker;
        private struct ExitMarker;
        private struct StartDepthMarker;

        //      A
        //     / \
        //    B   C
        //   / \
        //  D   E      (WalkGraphs.Tree)
        [Fact]
        public void Enter_FiresTheHooksEnterInPreOrderWithEachNodesDepth()
        {
            WalkWithHooks<EnterMarker>(WalkGraphs.Tree(), 0);

            Assert.Equal(
                new[] { ("A", 0), ("B", 1), ("D", 2), ("E", 2), ("C", 1) },
                RecordingEnterExitHooks<EnterMarker>.Entered);
        }

        [Fact]
        public void Exit_FiresTheHooksExitInPostOrderWithEachNodesDepth()
        {
            WalkWithHooks<ExitMarker>(WalkGraphs.Tree(), 0);

            Assert.Equal(
                new[] { ("D", 2), ("E", 2), ("B", 1), ("C", 1), ("A", 0) },
                RecordingEnterExitHooks<ExitMarker>.Exited);
        }

        [Fact]
        public void Enter_AndExit_ReportDepthRelativeToTheStartingDepth()
        {
            WalkWithHooks<StartDepthMarker>(WalkGraphs.Tree(), 3);

            Assert.Equal(
                new[] { ("A", 3), ("B", 4), ("D", 5), ("E", 5), ("C", 4) },
                RecordingEnterExitHooks<StartDepthMarker>.Entered);
            Assert.Equal(
                new[] { ("D", 5), ("E", 5), ("B", 4), ("C", 4), ("A", 3) },
                RecordingEnterExitHooks<StartDepthMarker>.Exited);
        }

        private static void WalkWithHooks<TMarker>(TestNode root, int startDepth)
            where TMarker : struct
            => DepthFirstWalk.Walk<
                TestNode, TestTopology, ListChildren<TestNode>,
                NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
                UnguardedVisit<TestNode>, RecordingEnterExitHooks<TMarker>>(root, startDepth, default);
    }
}
