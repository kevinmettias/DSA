using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.Tests.Domain.Locks;

public sealed class LockTopologyTests
{
    [Fact]
    public void GetChildren_ExposesTheCombinationsOneTurnAway()
    {
        var graph = LockGraph.Build([]);

        Assert.True(graph.TryGetNode("0000", out var node));

        var children = LockTopology.GetChildren(node);

        Assert.Equal(LockWheels.Count * 2, children.Count);
        Assert.All(
            Enumerable.Range(0, children.Count),
            i => Assert.Contains(children.Get(i).Combination, LockGraph.WheelTurnNeighbors("0000")));
    }

    [Fact]
    public void GetChildren_ReflectsDeadendPruning()
    {
        var graph = LockGraph.Build(["0001"]);

        Assert.True(graph.TryGetNode("0000", out var node));
        Assert.Equal(LockWheels.Count * 2 - 1, LockTopology.GetChildren(node).Count);
    }
}
