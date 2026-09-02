using DSAExperimentation.LeetCode.RangeSumQuery2DImmutable;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumQuery2DImmutable;

// Harness only. Both strategies are RangeSumQuery2DImmutableSolution's - this file just pins them
// to LeetCode's published examples, one [Theory] per strategy so a failure names the strategy that
// broke.
public sealed class RangeSumQuery2DImmutableTests
{
    private static readonly int[][] ExampleMatrix =
    [
        [3, 0, 1, 4, 2],
        [5, 6, 3, 2, 1],
        [1, 2, 0, 1, 5],
        [4, 1, 0, 1, 7],
        [1, 0, 3, 0, 5],
    ];

    public static TheoryData<int, int, int, int, int> Examples =>
        new()
        {
            { 2, 1, 4, 3, 8 },
            { 1, 1, 2, 2, 11 },
            { 1, 2, 2, 4, 12 },
            { 1, 1, 1, 1, 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBruteForceCellScan_LeetCodeExamples_ReturnsRegionSum(
        int row1, int col1, int row2, int col2, int expected)
    {
        var numMatrix = RangeSumQuery2DImmutableSolution.CreateByBruteForceCellScan(ExampleMatrix);

        Assert.Equal(expected, numMatrix.SumRegion(row1, col1, row2, col2));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByRowFenwickTree_LeetCodeExamples_ReturnsRegionSum(
        int row1, int col1, int row2, int col2, int expected)
    {
        var numMatrix = RangeSumQuery2DImmutableSolution.CreateByRowFenwickTree(ExampleMatrix);

        Assert.Equal(expected, numMatrix.SumRegion(row1, col1, row2, col2));
    }
}
