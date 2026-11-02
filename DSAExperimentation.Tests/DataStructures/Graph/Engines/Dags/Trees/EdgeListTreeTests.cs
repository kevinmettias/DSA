using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

// Expected values are derived by hand on one five-node tree, whose edges are written in both
// directions so that reading an edge the wrong way round would show:
// edge 0 joins 1-0, edge 1 joins 1-2, edge 2 joins 3-1 and edge 3 joins 0-4.
public sealed partial class EdgeListTreeTests
{
    private static readonly int[][] SampleEdges = [[1, 0], [1, 2], [3, 1], [0, 4]];

    // The same tree numbered from 1, with a weight in a third column the walk must ignore.
    private static readonly int[][] OneBasedSampleEdges = [[2, 1, 7], [2, 3, 8], [4, 2, 9], [1, 5, 6]];

    private const int SampleNodeCount = 5;

    // A chain this long is past LeetCode's 10^5 nodes and deep enough to overflow a recursive walk.
    private const int ChainLength = 100_001;

    [Fact]
    public void ParentsFrom_SampleTreeRootedAtZero_GivesEachNodeItsParentAndJoiningEdge()
    {
        var (parent, parentEdge) = EdgeListTree.ParentsFrom(SampleNodeCount, SampleEdges, root: 0, firstId: 0);

        Assert.Equal([-1, 0, 1, 1, 0], parent);
        Assert.Equal([-1, 0, 1, 2, 3], parentEdge);
    }

    // Hung from node 2, edge 1 now points the other way: node 1 sits below node 2.
    [Fact]
    public void ParentsFrom_SampleTreeRootedAtALeaf_HangsEveryEdgeBelowThatLeaf()
    {
        var (parent, parentEdge) = EdgeListTree.ParentsFrom(SampleNodeCount, SampleEdges, root: 2, firstId: 0);

        Assert.Equal([1, 2, -1, 1, 0], parent);
        Assert.Equal([0, 1, -1, 2, 3], parentEdge);
    }

    [Fact]
    public void ParentsFrom_EdgesNumberedFromOne_ReturnsIdsNumberedFromZero()
    {
        var (parent, parentEdge) = EdgeListTree.ParentsFrom(SampleNodeCount, OneBasedSampleEdges, root: 0, firstId: 1);

        Assert.Equal([-1, 0, 1, 1, 0], parent);
        Assert.Equal([-1, 0, 1, 2, 3], parentEdge);
    }

    [Fact]
    public void ParentsFrom_EdgesThatLeaveANodeUnconnected_ThrowsArgumentException()
    {
        int[][] twoComponents = [[0, 1], [2, 3]];

        Assert.Throws<ArgumentException>(() => EdgeListTree.ParentsFrom(4, twoComponents, root: 0, firstId: 0));
    }

    // A recursive walk would need one frame per node here.
    [Fact]
    public void ParentsFrom_ChainPastLeetCodesSize_HangsEachNodeFromItsPredecessor()
    {
        var chain = Enumerable.Range(0, ChainLength - 1).Select(id => new[] { id, id + 1 }).ToArray();

        var (parent, parentEdge) = EdgeListTree.ParentsFrom(ChainLength, chain, root: 0, firstId: 0);

        Assert.Equal(ChainLength - 2, parent[ChainLength - 1]);
        Assert.Equal(ChainLength - 2, parentEdge[ChainLength - 1]);
    }
}
