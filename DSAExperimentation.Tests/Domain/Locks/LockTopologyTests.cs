using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.Tests.Domain.Locks;

public sealed partial class LockTopologyTests
{
    [Fact]
    public void GetChildren_ExposesTheCombinationsOneTurnAway()
    {
        var node = NodeIn("0000", []);

        var children = LockTopology.GetChildren(node);

        Assert.Equal(LockWheels.Count * 2, children.Count);
        AssertEachChildIsOneWheelTurnAway(children);
    }

    [Fact]
    public void GetChildren_ReflectsDeadendPruning()
    {
        var node = NodeIn("0000", ["0001"]);

        Assert.Equal(LockWheels.Count * 2 - 1, LockTopology.GetChildren(node).Count);
    }

    // The arrangement both tests open with - build the graph from the deadends, find
    // the start combination in it, and insist it was reachable - named once so the node
    // it yields threads into every assertion below instead of being re-found in each body.
    private static LockNode NodeIn(string combination, string[] deadends)
    {
        var graph = LockGraph.Build(deadends);

        var found = graph.TryGetNode(combination, out var node);
        Assert.True(found);

        return node;
    }

    // The per-child half of the count the first test asserts: every combination the
    // topology hands back is one wheel turn from the "0000" the graph was seeded with.
    // IChildren is a Get-by-index view, not a sequence, so the walk is over indices.
    private static void AssertEachChildIsOneWheelTurnAway(ListChildren<LockNode> children)
    {
        var indices = Enumerable.Range(0, children.Count);
        var oneTurnAway = LockGraph.WheelTurnNeighbors("0000");

        Assert.All(
            indices,
            i => Assert.Contains(children.Get(i).Combination, oneTurnAway));
    }
}
