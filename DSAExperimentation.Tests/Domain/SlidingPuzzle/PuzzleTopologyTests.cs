using DSAExperimentation.Domain.SlidingPuzzle;

namespace DSAExperimentation.Tests.Domain.SlidingPuzzle;

public sealed class PuzzleTopologyTests
{
    [Fact]
    public void GetChildren_ExposesTheBoardsOneBlankSlideAway()
    {
        var graph = PuzzleGraph.Build();

        Assert.True(graph.TryGetNode("123450", out var node));

        var children = PuzzleTopology.GetChildren(node);

        Assert.Equal(2, children.Count);
        Assert.All(
            Enumerable.Range(0, children.Count),
            i => Assert.Contains(children.Get(i).State, PuzzleGraph.BlankSlideNeighbors("123450")));
    }

    [Fact]
    public void GetChildren_BlankOnAnEdge_ReturnsThreeChildren()
    {
        var graph = PuzzleGraph.Build();

        Assert.True(graph.TryGetNode("102345", out var node));
        Assert.Equal(3, PuzzleTopology.GetChildren(node).Count);
    }
}
