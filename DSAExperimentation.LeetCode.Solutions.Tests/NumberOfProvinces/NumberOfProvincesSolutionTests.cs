using DSAExperimentation.LeetCode.Solutions.NumberOfProvinces;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfProvinces;

// Harness only: the algorithms live in NumberOfProvincesSolution. One test method
// per strategy over one shared set of cases, so a failure names the strategy that
// broke; the case here is a province that is only connected transitively.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class NumberOfProvincesSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            {
                // City 0 connects only to 1, and 1 only to 2 - no direct 0-2 edge,
                // so the province still has to be discovered transitively through
                // Union's chaining.
                [
                    [1, 1, 0],
                    [1, 1, 1],
                    [0, 1, 1],
                ],
                1
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountProvincesByDepthFirstFloodFill_Cases_ReturnsProvinceCount(
        int[][] isConnected, int expected) =>
        Assert.Equal(expected, NumberOfProvincesSolution.CountProvincesByDepthFirstFloodFill(isConnected));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountProvincesByDisjointSetUnionFind_Cases_ReturnsProvinceCount(
        int[][] isConnected, int expected) =>
        Assert.Equal(expected, NumberOfProvincesSolution.CountProvincesByDisjointSetUnionFind(isConnected));
}
