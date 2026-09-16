using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.Walking.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking;

// Harness only. Walk and NextLevel are the two halves of the one breadth-first
// engine - Walk is what calls NextLevel once per depth - so every case names its
// graph in one vocabulary and the whole file reaches the engine through Build/
// Walk/NextLevel below rather than one builder per operation. A shape that
// repeats a node (a cycle, or two parents sharing a child) walks under the
// tracked guard, because an unguarded walk is valid only under ITreeTopology's
// unique-ancestry promise; the depths and the frontier row show what the guard
// does to the visit order.
public sealed class BreadthFirstWalkTests
{
    public static TheoryData<WalkExample> Examples =>
        new()
        {
            { new WalkExample(GraphShape.Tree, ["A", "B", "C", "D", "E"], [0, 1, 1, 2, 2], ["B", "C"]) },
            { new WalkExample(GraphShape.Single, ["A"], [0], []) },
            { new WalkExample(GraphShape.Cycle, ["A", "B", "C"], [0, 1, 2], ["B"]) },
            { new WalkExample(GraphShape.Diamond, ["A", "B", "C", "D"], [0, 1, 1, 2], ["B", "C"]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void Walk_VisitsEveryNodeOnceLevelByLevel(WalkExample example)
    {
        var visited = Walk(example.Shape);

        Assert.Equal(example.Names, visited.Select(v => v.Name));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void Walk_ReportsTheDepthOfEachNode(WalkExample example)
    {
        var visited = Walk(example.Shape);

        Assert.Equal(example.Depths, visited.Select(v => v.Depth));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NextLevel_ReturnsTheChildrenOfTheWholeFrontierInOrder(WalkExample example)
    {
        var next = NextLevel(example.Shape);

        Assert.Equal(example.Frontier, next.Select(n => n.Name));
    }

    // The guard is applied to the whole frontier, not just to the root: the second
    // level of the diamond is the shared node alone, reached twice and visited once.
    [Fact]
    public void NextLevel_AppliesTheGuardSoASharedChildAppearsOnce()
    {
        var (root, shared) = WalkGraphs.DiamondShare();
        var guard = new TrackedVisitGuard<TestNode>([root]);

        var level1 = NextLevelWith([root], guard);
        var level2 = NextLevelWith(level1, guard);

        Assert.Equal([shared], level2);
    }

    [Fact]
    public void NextLevel_RespectsTheChildOrderWitness()
    {
        var root = Build(GraphShape.Tree);

        var next = NextLevelReversed([root], new UnguardedVisit<TestNode>());

        Assert.Equal(["C", "B"], next.Select(n => n.Name));
    }

    private static List<(string Name, int Depth)> Walk(GraphShape shape)
    {
        var root = Build(shape);
        var guard = GuardFor(root, shape);

        return WalkWith(root, guard);
    }

    private static List<TestNode> NextLevel(GraphShape shape)
    {
        var root = Build(shape);
        var guard = GuardFor(root, shape);

        return NextLevelWith([root], guard);
    }

    private static List<(string Name, int Depth)> WalkWith<TGuard>(TestNode root, TGuard guard)
        where TGuard : IVisitGuard<TestNode> =>
        BreadthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TGuard, RecordingWalkStep, List<(string, int)>>(
            root, [], guard);

    private static List<TestNode> NextLevelWith<TGuard>(List<TestNode> currentLevel, TGuard guard)
        where TGuard : IVisitGuard<TestNode> =>
        BreadthFirstWalk.NextLevel<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TGuard>(currentLevel, guard);

    private static List<TestNode> NextLevelReversed<TGuard>(List<TestNode> currentLevel, TGuard guard)
        where TGuard : IVisitGuard<TestNode> =>
        BreadthFirstWalk.NextLevel<
            TestNode, TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            TGuard>(currentLevel, guard);

    // The graph-safe guard is seeded with the root, which is already visited by
    // definition; the tree-only shapes keep the unguarded visit they can rely on.
    private static IVisitGuard<TestNode> GuardFor(TestNode root, GraphShape shape)
    {
        if (NeedsTrackedGuard(shape))
        {
            return new TrackedVisitGuard<TestNode>([root]);
        }

        return new UnguardedVisit<TestNode>();
    }

    private static bool NeedsTrackedGuard(GraphShape shape) =>
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

    public readonly record struct WalkExample(GraphShape Shape, string[] Names, int[] Depths, string[] Frontier);
}
