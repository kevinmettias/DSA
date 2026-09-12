using DSAExperimentation.Domain.SlidingPuzzle;

namespace DSAExperimentation.Tests.Domain.SlidingPuzzle;

public sealed class PuzzleGraphTests
{
    [Fact]
    public void BlankSlideNeighbors_BlankInACorner_ReturnsTwoNeighbors()
    {
        var neighbors = PuzzleGraph.BlankSlideNeighbors("123450").ToList();

        Assert.Equal(2, neighbors.Count);
        Assert.Contains("120453", neighbors);
        Assert.Contains("123405", neighbors);
    }

    [Fact]
    public void BlankSlideNeighbors_BlankOnAnEdge_ReturnsThreeNeighbors()
    {
        var neighbors = PuzzleGraph.BlankSlideNeighbors("102345").ToList();

        Assert.Equal(3, neighbors.Count);
    }

    [Fact]
    public void BlankSlideNeighbors_EachResultMovesTheBlankExactlyOnePosition()
    {
        const string state = "123450";
        var blank = state.IndexOf('0');

        foreach (var neighbor in PuzzleGraph.BlankSlideNeighbors(state))
        {
            var differing = Enumerable.Range(0, state.Length).Where(i => neighbor[i] != state[i]).ToList();

            Assert.Equal(2, differing.Count);
            Assert.Contains(blank, differing);
        }
    }

    [Fact]
    public void BlankSlideNeighbors_IsSymmetric()
    {
        foreach (var neighbor in PuzzleGraph.BlankSlideNeighbors("123450"))
        {
            Assert.Contains("123450", PuzzleGraph.BlankSlideNeighbors(neighbor));
        }
    }

    [Fact]
    public void Build_ContainsEveryPermutationOfTheSixCellBoard()
    {
        var graph = PuzzleGraph.Build();

        Assert.True(graph.TryGetNode("123450", out _));
        Assert.True(graph.TryGetNode("054321", out _));
        Assert.True(graph.TryGetNode("012345", out _));
    }

    [Fact]
    public void Build_GivesACornerBlankNodeTwoNeighbors()
    {
        var graph = PuzzleGraph.Build();

        Assert.True(graph.TryGetNode("123450", out var node));
        Assert.Equal(2, node.Neighbors.Count);
    }

    [Fact]
    public void Build_GivesAnEdgeBlankNodeThreeNeighbors()
    {
        var graph = PuzzleGraph.Build();

        Assert.True(graph.TryGetNode("102345", out var node));
        Assert.Equal(3, node.Neighbors.Count);
    }

    [Fact]
    public void Build_NeverWiresANodeToItself()
    {
        var graph = PuzzleGraph.Build();

        Assert.True(graph.TryGetNode("123450", out var node));
        Assert.DoesNotContain(node.Neighbors, n => n.State == "123450");
    }

    [Fact]
    public void TryGetNode_StateOutsideThePermutationSpace_ReturnsFalse()
    {
        var graph = PuzzleGraph.Build();

        Assert.False(graph.TryGetNode("not-a-state", out _));
    }
}
