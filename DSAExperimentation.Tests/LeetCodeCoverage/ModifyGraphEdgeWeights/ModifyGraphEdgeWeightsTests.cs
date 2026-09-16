using DSAExperimentation.LeetCode.ModifyGraphEdgeWeights;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ModifyGraphEdgeWeights;

// Harness only. Both strategies are ModifyGraphEdgeWeightsSolution's - the linear
// weight scan and the half-distance formula - and this file pins both to
// LeetCode's three published examples plus the three the pre-migration test
// carried: the floor already on target (so a -1 edge keeps weight 1), the floor
// already above it (impossible), and a plain single-edge stretch.
//
// LeetCode accepts any assignment whose shortest source-to-destination distance
// is exactly target, so its own sample output for the first example differs from
// the one below; every expected array here is what BOTH strategies produce, and
// each has been checked by hand to put the shortest distance exactly on target.
public sealed partial class ModifyGraphEdgeWeightsTests
{
    public static TheoryData<WeightEditExample> Examples =>
        new()
        {
            {
                new WeightEditExample(
                    N: 5,
                    Edges: [[4, 1, -1], [2, 0, -1], [0, 3, -1], [4, 3, -1]],
                    Source: 0,
                    Destination: 1,
                    Target: 5,
                    Expected: [[4, 1, 3], [2, 0, 1], [0, 3, 1], [4, 3, 1]])
            },
            { new WeightEditExample(N: 3, Edges: [[0, 1, -1], [0, 2, 5]], Source: 0, Destination: 2, Target: 6, Expected: []) },
            {
                new WeightEditExample(
                    N: 4,
                    Edges: [[1, 0, 4], [1, 2, 3], [2, 3, 5], [0, 3, -1]],
                    Source: 0,
                    Destination: 2,
                    Target: 6,
                    Expected: [[1, 0, 4], [1, 2, 3], [2, 3, 5], [0, 3, 1]])
            },
            {
                new WeightEditExample(
                    N: 3, Edges: [[0, 1, -1], [1, 2, 4]], Source: 0, Destination: 2, Target: 10,
                    Expected: [[0, 1, 6], [1, 2, 4]])
            },
            {
                new WeightEditExample(
                    N: 3, Edges: [[0, 1, -1], [1, 2, 1]], Source: 0, Destination: 2, Target: 2,
                    Expected: [[0, 1, 1], [1, 2, 1]])
            },
            { new WeightEditExample(N: 3, Edges: [[0, 1, -1], [0, 2, 3]], Source: 0, Destination: 2, Target: 2, Expected: []) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModifyEdgeWeightsByLinearWeightScan_LeetCodeExamples_PutsTheShortestDistanceOnTarget(
        WeightEditExample example)
    {
        var actual = ModifyGraphEdgeWeightsSolution.ModifyEdgeWeightsByLinearWeightScan(
            example.N,
            example.Edges,
            (example.Source, example.Destination, example.Target));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModifyEdgeWeightsByHalfDistanceFormula_LeetCodeExamples_PutsTheShortestDistanceOnTarget(
        WeightEditExample example)
    {
        var actual = ModifyGraphEdgeWeightsSolution.ModifyEdgeWeightsByHalfDistanceFormula(
            example.N,
            example.Edges,
            (example.Source, example.Destination, example.Target));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the graph with its -1 weights, the query's three vertices,
    // and the reweighted edge list. The vertices are all `int`, so the fields name
    // source, destination and target rather than leaving three adjacent positions a
    // caller could swap.
    public readonly record struct WeightEditExample(
        int N,
        int[][] Edges,
        int Source,
        int Destination,
        int Target,
        int[][] Expected);
}
