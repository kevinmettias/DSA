using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
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
}
