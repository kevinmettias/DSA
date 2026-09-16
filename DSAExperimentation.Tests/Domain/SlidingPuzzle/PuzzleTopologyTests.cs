using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Domain.SlidingPuzzle;

namespace DSAExperimentation.Tests.Domain.SlidingPuzzle;

public sealed class PuzzleTopologyTests
{
    [Fact]
    public void GetChildren_ExposesTheBoardsOneBlankSlideAway()
    {
        var node = NodeIn("123450");

        var children = PuzzleTopology.GetChildren(node);

        Assert.Equal(2, children.Count);
        AssertEachChildIsOneBlankSlideAway(children);
    }

    [Fact]
    public void GetChildren_BlankOnAnEdge_ReturnsThreeChildren()
    {
        var node = NodeIn("102345");

        Assert.Equal(3, PuzzleTopology.GetChildren(node).Count);
    }

    // The arrangement both tests open with - build the full board graph, find the
    // state in it, and insist it was reachable - named once so the node it yields
    // threads into every assertion below instead of being re-found in each body.
    private static PuzzleNode NodeIn(string state)
    {
        var graph = PuzzleGraph.Build();

        var found = graph.TryGetNode(state, out var node);
        Assert.True(found);

        return node;
    }

    // The per-child half of the count the first test asserts: every board the topology
    // hands back is one blank slide from the solved "123450" the graph was seeded with.
    // IChildren is a Get-by-index view, not a sequence, so the walk is over indices.
    private static void AssertEachChildIsOneBlankSlideAway(ListChildren<PuzzleNode> children)
    {
        var indices = Enumerable.Range(0, children.Count);
        var oneBlankSlideAway = PuzzleGraph.BlankSlideNeighbors("123450");

        Assert.All(
            indices,
            i => Assert.Contains(children.Get(i).State, oneBlankSlideAway));
    }
}
