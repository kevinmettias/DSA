using DSAExperimentation.LeetCode.MinCostToConnectAllPoints;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinCostToConnectAllPoints;

// Harness only. Both the dense-Prim baseline and the Kruskal composition are
// MinCostToConnectAllPointsSolution's own methods; this file just pins them to
// LeetCode's published examples plus the degenerate one- and two-point inputs.
// The baseline used to live inlined in MinCostToConnectAllPointsBenchmarks and was
// asserted by nothing - it is under test here for the first time.
public sealed class MinCostToConnectAllPointsTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 0], [2, 2], [3, 10], [5, 2], [7, 0]], 20 },
            { [[3, 12], [-2, 5], [-4, 1]], 18 },
            { [[0, 0]], 0 },
            { [[0, 0], [1, 1]], 2 },
            { [[0, 0], [1, 1], [2, 2]], 4 },
            { [[-1000000, -1000000], [1000000, 1000000]], 4000000 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostConnectPointsByDensePrim_LeetCodeExamples_ReturnsSpanningTreeWeight(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinCostToConnectAllPointsSolution.MinCostConnectPointsByDensePrim(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostConnectPointsByKruskalMst_LeetCodeExamples_ReturnsSpanningTreeWeight(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinCostToConnectAllPointsSolution.MinCostConnectPointsByKruskalMst(points));
}
