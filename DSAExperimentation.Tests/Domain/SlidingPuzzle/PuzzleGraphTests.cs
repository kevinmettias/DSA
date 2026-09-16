using DSAExperimentation.Domain.SlidingPuzzle;

namespace DSAExperimentation.Tests.Domain.SlidingPuzzle;

public sealed class PuzzleGraphTests
{
    // The solved board the blank-slide tests are stated against. A constant rather
    // than a local in the one test that needs the blank's own position: its scope is
    // a claim about where the value is authoritative, and that is this type, not the
    // body of a single method.
    private const string State = "123450";

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
        var blank = State.IndexOf('0');

        foreach (var neighbor in PuzzleGraph.BlankSlideNeighbors(State))
        {
            var differing = Enumerable.Range(0, State.Length).Where(i => neighbor[i] != State[i]).ToList();

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
        var solved = graph.TryGetNode("123450", out _);
        var reversed = graph.TryGetNode("054321", out _);
        var rotated = graph.TryGetNode("012345", out _);

        Assert.True(solved);
        Assert.True(reversed);
        Assert.True(rotated);
    }

    [Fact]
    public void Build_GivesACornerBlankNodeTwoNeighbors()
    {
        var graph = PuzzleGraph.Build();
        var found = graph.TryGetNode("123450", out var node);

        Assert.True(found);
        Assert.Equal(2, node.Neighbors.Count);
    }

    [Fact]
    public void Build_GivesAnEdgeBlankNodeThreeNeighbors()
    {
        var graph = PuzzleGraph.Build();
        var found = graph.TryGetNode("102345", out var node);

        Assert.True(found);
        Assert.Equal(3, node.Neighbors.Count);
    }

    [Fact]
    public void Build_NeverWiresANodeToItself()
    {
        var graph = PuzzleGraph.Build();
        var found = graph.TryGetNode("123450", out var node);

        Assert.True(found);
        Assert.DoesNotContain(node.Neighbors, n => n.State == "123450");
    }

    [Fact]
    public void TryGetNode_StateOutsideThePermutationSpace_ReturnsFalse()
    {
        var graph = PuzzleGraph.Build();
        var found = graph.TryGetNode("not-a-state", out _);

        Assert.False(found);
    }
}
