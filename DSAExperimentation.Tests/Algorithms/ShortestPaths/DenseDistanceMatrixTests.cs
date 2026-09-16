using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

// The two construction steps AllPairsShortestPaths' Floyd-Warshall walk starts from: the
// vertex-to-row index, and the V-by-V grid carrying zero on the diagonal, the "still
// unreached" sentinel elsewhere, and each graph's own direct edges in its row. Pinned on the
// same sample graph the walk's own tests use (A-[1]->B, A-[4]->C, B-[2]->C, B-[5]->D,
// C-[1]->D), so a wrong index or a missing seed fails under the name of the step that made
// it, before any refinement the walk itself owns can hide it.
public sealed partial class DenseDistanceMatrixTests
{
    [Fact]
    public void BuildIndex_MapsEveryVertexToItsPositionInTheVertexList()
    {
        var (a, b, c, d) = WeightedGraphs.SampleGraph();
        var vertices = new List<WeightedNode> { a, b, c, d };

        var index = DenseDistanceMatrix.BuildIndex(vertices);

        Assert.Equal(0, index[a]);
        Assert.Equal(1, index[b]);
        Assert.Equal(2, index[c]);
        Assert.Equal(3, index[d]);
        Assert.Equal(vertices.Count, index.Count);
    }

    [Fact]
    public void BuildInitialMatrix_SeedsZeroOnTheDiagonalEachDirectEdgeAndTheSentinelElsewhere()
    {
        var (a, b, c, d) = WeightedGraphs.SampleGraph();
        var vertices = new List<WeightedNode> { a, b, c, d };
        var index = DenseDistanceMatrix.BuildIndex(vertices);

        var matrix = DenseDistanceMatrix.BuildInitialMatrix<
            WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(vertices, index);

        // Every vertex's own cell, not a sampled pair: the zero the walk's first refinement
        // step relies on has to hold for all four rows.
        Assert.All(vertices, vertex => Assert.Equal(0, matrix[index[vertex], index[vertex]]));

        Assert.Equal(1, matrix[index[a], index[b]]);
        Assert.Equal(4, matrix[index[a], index[c]]);
        Assert.Equal(2, matrix[index[b], index[c]]);
        Assert.Equal(5, matrix[index[b], index[d]]);
        Assert.Equal(1, matrix[index[c], index[d]]);

        // Nothing reaches A, and D reaches nothing, so both cells keep the "still unreached"
        // sentinel rather than a zero that would read as a free hop.
        Assert.Equal(int.MaxValue, matrix[index[b], index[a]]);
        Assert.Equal(int.MaxValue, matrix[index[d], index[a]]);
    }
}
