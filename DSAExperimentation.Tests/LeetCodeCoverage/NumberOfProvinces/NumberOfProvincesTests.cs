using DSAExperimentation.LeetCode.NumberOfProvinces;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfProvinces;

// Harness only: the algorithms live in NumberOfProvincesSolution. One test method
// per strategy over one shared set of LeetCode's own examples, so a failure names
// the strategy that broke.
public sealed partial class NumberOfProvincesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [
                    [1, 1, 0],
                    [1, 1, 0],
                    [0, 0, 1],
                ],
                2
            },
            {
                [
                    [1, 0, 0],
                    [0, 1, 0],
                    [0, 0, 1],
                ],
                3
            },
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
    [MemberData(nameof(Examples))]
    public void CountProvincesByDepthFirstFloodFill_LeetCodeExamples_ReturnsProvinceCount(
        int[][] isConnected, int expected) =>
        Assert.Equal(expected, NumberOfProvincesSolution.CountProvincesByDepthFirstFloodFill(isConnected));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountProvincesByDisjointSetUnionFind_LeetCodeExamples_ReturnsProvinceCount(
        int[][] isConnected, int expected) =>
        Assert.Equal(expected, NumberOfProvincesSolution.CountProvincesByDisjointSetUnionFind(isConnected));
}
