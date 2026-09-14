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
public sealed class ModifyGraphEdgeWeightsTests
{
    public static TheoryData<int, int[][], int, int, int, int[][]> Examples =>
        new()
        {
            {
                5, [[4, 1, -1], [2, 0, -1], [0, 3, -1], [4, 3, -1]], 0, 1, 5,
                [[4, 1, 3], [2, 0, 1], [0, 3, 1], [4, 3, 1]]
            },
            { 3, [[0, 1, -1], [0, 2, 5]], 0, 2, 6, [] },
            {
                4, [[1, 0, 4], [1, 2, 3], [2, 3, 5], [0, 3, -1]], 0, 2, 6,
                [[1, 0, 4], [1, 2, 3], [2, 3, 5], [0, 3, 1]]
            },
            { 3, [[0, 1, -1], [1, 2, 4]], 0, 2, 10, [[0, 1, 6], [1, 2, 4]] },
            { 3, [[0, 1, -1], [1, 2, 1]], 0, 2, 2, [[0, 1, 1], [1, 2, 1]] },
            { 3, [[0, 1, -1], [0, 2, 3]], 0, 2, 2, [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModifyEdgeWeightsByLinearWeightScan_LeetCodeExamples_PutsTheShortestDistanceOnTarget(
        int n, int[][] edges, int source, int destination, int target, int[][] expected) =>
        Assert.Equal(
            expected,
            ModifyGraphEdgeWeightsSolution.ModifyEdgeWeightsByLinearWeightScan(
                n, edges, source, destination, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModifyEdgeWeightsByHalfDistanceFormula_LeetCodeExamples_PutsTheShortestDistanceOnTarget(
        int n, int[][] edges, int source, int destination, int target, int[][] expected) =>
        Assert.Equal(
            expected,
            ModifyGraphEdgeWeightsSolution.ModifyEdgeWeightsByHalfDistanceFormula(
                n, edges, source, destination, target));
}
