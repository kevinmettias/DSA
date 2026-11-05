using DSAExperimentation.LeetCode.Solutions.MinCostToConnectAllPoints;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinCostToConnectAllPoints;

// Harness only. Both the dense-Prim baseline and the Kruskal composition are
// MinCostToConnectAllPointsSolution's own methods; this file just pins them to
// the degenerate one- and two-point inputs.
// The baseline used to live inlined in MinCostToConnectAllPointsBenchmarks and was
// asserted by nothing - it is under test here for the first time.
public sealed partial class MinCostToConnectAllPointsSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0, 0]], 0 },
            { [[0, 0], [1, 1]], 2 },
            { [[0, 0], [1, 1], [2, 2]], 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCostConnectPointsByDensePrim_Cases_ReturnsSpanningTreeWeight(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinCostToConnectAllPointsSolution.MinCostConnectPointsByDensePrim(points));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCostConnectPointsByKruskalMst_Cases_ReturnsSpanningTreeWeight(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinCostToConnectAllPointsSolution.MinCostConnectPointsByKruskalMst(points));
}
