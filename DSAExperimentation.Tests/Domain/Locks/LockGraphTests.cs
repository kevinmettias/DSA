using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.Tests.Domain.Locks;

public sealed partial class LockGraphTests
{
    [Fact]
    public void WheelTurnNeighbors_ReturnsTwoTurnsPerWheel()
    {
        var neighbours = LockGraph.WheelTurnNeighbors("0000").ToList();

        Assert.Equal(LockWheels.Count * 2, neighbours.Count);
    }

    [Fact]
    public void WheelTurnNeighbors_TurnsExactlyOneWheelByOneClick()
    {
        var neighbours = LockGraph.WheelTurnNeighbors("5555");

        Assert.All(neighbours, neighbour =>
        {
            var differing = Enumerable.Range(0, LockWheels.Count).Where(i => neighbour[i] != '5').ToList();
            Assert.Single(differing);
            Assert.Contains(neighbour[differing[0]], "46");
        });
    }

    [Fact]
    public void WheelTurnNeighbors_WrapsAroundZeroAndNine()
    {
        Assert.Contains("9000", LockGraph.WheelTurnNeighbors("0000"));
        Assert.Contains("0000", LockGraph.WheelTurnNeighbors("9000"));
    }

    [Fact]
    public void WheelTurnNeighbors_IsSymmetric()
    {
        foreach (var neighbour in LockGraph.WheelTurnNeighbors("1234"))
        {
            Assert.Contains("1234", LockGraph.WheelTurnNeighbors(neighbour));
        }
    }

    [Fact]
    public void Build_WithNoDeadends_ContainsTheWholeCombinationSpace()
    {
        var graph = LockGraph.Build([]);
        var hasZeroes = graph.TryGetNode("0000", out _);
        var hasNines = graph.TryGetNode("9999", out _);
        var hasFives = graph.TryGetNode("5555", out _);

        Assert.True(hasZeroes);
        Assert.True(hasNines);
        Assert.True(hasFives);
    }

    [Fact]
    public void Build_GivesEveryLiveCombinationEightNeighbours()
    {
        var graph = LockGraph.Build([]);
        var found = graph.TryGetNode("1234", out var node);

        Assert.True(found);
        Assert.Equal(LockWheels.Count * 2, node.Neighbors.Count);
    }

    [Fact]
    public void Build_OmitsDeadendsEntirely()
    {
        var graph = LockGraph.Build(["0001"]);
        var found = graph.TryGetNode("0001", out _);

        Assert.False(found);
    }

    [Fact]
    public void Build_NeverWiresAnEdgeIntoADeadend()
    {
        var graph = LockGraph.Build(["0001"]);
        var found = graph.TryGetNode("0000", out var start);

        Assert.True(found);
        Assert.DoesNotContain(start.Neighbors, n => n.Combination == "0001");
        Assert.Equal(LockWheels.Count * 2 - 1, start.Neighbors.Count);
    }

    [Fact]
    public void TryGetNode_CombinationOutsideTheSpace_ReturnsFalse()
    {
        var graph = LockGraph.Build([]);
        var found = graph.TryGetNode("nope", out _);

        Assert.False(found);
    }
}
