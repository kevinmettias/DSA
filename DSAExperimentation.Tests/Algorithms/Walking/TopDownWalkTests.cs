using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.Traversal.TopDown.Fixtures;
using DSAExperimentation.Tests.Algorithms.Walking.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking;

public sealed class TopDownWalkTests
{
    private static List<(string Path, int Depth, NodePosition Position)> Walk(TestNode root, int depth)
    {
        var recorded = new List<(string, int, NodePosition)>();

        TopDownWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            UnguardedVisit<TestNode>, RecordingPathHooks, RecordingPathHooks.PathSoFar>(
            root, new RecordingPathHooks.PathSoFar([root.Name], recorded), depth, new UnguardedVisit<TestNode>());

        return recorded;
    }

    [Fact]
    public void Walk_VisitsTheParentBeforeAnyOfItsChildren()
    {
        var visited = Walk(WalkGraphs.Tree(), 0);

        Assert.Equal(["A", "A/B", "A/B/D", "A/B/E", "A/C"], visited.Select(v => v.Path));
    }

    [Fact]
    public void Walk_HonoursTheStartingDepth()
    {
        var visited = Walk(new TestNode("A"), 5);

        Assert.Equal([("A", 5, NodePosition.Leaf)], visited);
    }

    [Fact]
    public void Walk_ClassifiesAChildlessNodeAsALeaf()
    {
        var visited = Walk(WalkGraphs.Tree(), 0);

        Assert.Equal(NodePosition.Leaf, visited.Single(v => v.Path == "A/C").Position);
    }

    [Fact]
    public void Walk_ClassifiesANodeWithChildrenAsInterior()
    {
        var visited = Walk(WalkGraphs.Tree(), 0);

        Assert.Equal(NodePosition.Interior, visited.Single(v => v.Path == "A/B").Position);
    }

    [Fact]
    public void Walk_TrackedGuardOnACycle_Terminates()
    {
        var root = WalkGraphs.Cycle();
        var recorded = new List<(string, int, NodePosition)>();

        TopDownWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TrackedVisitGuard<TestNode>, RecordingPathHooks, RecordingPathHooks.PathSoFar>(
            root, new RecordingPathHooks.PathSoFar([root.Name], recorded), 0, new TrackedVisitGuard<TestNode>([root]));

        Assert.Equal(["A", "A/B", "A/B/C"], recorded.Select(r => r.Item1));
    }

    [Fact]
    public void Walk_RespectsTheChildOrderWitness()
    {
        var root = WalkGraphs.Tree();
        var recorded = new List<(string, int, NodePosition)>();

        TopDownWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            UnguardedVisit<TestNode>, RecordingPathHooks, RecordingPathHooks.PathSoFar>(
            root, new RecordingPathHooks.PathSoFar([root.Name], recorded), 0, new UnguardedVisit<TestNode>());

        Assert.Equal(["A", "A/C", "A/B", "A/B/E", "A/B/D"], recorded.Select(r => r.Item1));
    }
}
