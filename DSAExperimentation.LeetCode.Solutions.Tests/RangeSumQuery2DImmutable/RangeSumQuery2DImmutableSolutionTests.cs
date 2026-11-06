using DSAExperimentation.LeetCode.Solutions.RangeSumQuery2DImmutable;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RangeSumQuery2DImmutable;

// Harness only. All three strategies are RangeSumQuery2DImmutableSolution's. The rows are summed by
// hand from LeetCode's example matrix - one cell, the whole matrix (rows 10 + 17 + 9 + 13 + 9 = 58), the
// last column (2 + 1 + 5 + 7 + 5 = 20) and the last row (1 + 0 + 3 + 0 + 5 = 9), the regions that touch
// every edge of the prefix table. One [Theory] per strategy, so a failure names the strategy that broke.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class RangeSumQuery2DImmutableSolutionTests
{
    private static readonly int[][] ExampleMatrix =
    [
        [3, 0, 1, 4, 2],
        [5, 6, 3, 2, 1],
        [1, 2, 0, 1, 5],
        [4, 1, 0, 1, 7],
        [1, 0, 3, 0, 5],
    ];

    public static TheoryData<RegionSumCase> Cases =>
        new()
        {
            { new RegionSumCase(Row1: 1, Col1: 1, Row2: 1, Col2: 1, Expected: 6) },
            { new RegionSumCase(Row1: 0, Col1: 0, Row2: 4, Col2: 4, Expected: 58) },
            { new RegionSumCase(Row1: 0, Col1: 4, Row2: 4, Col2: 4, Expected: 20) },
            { new RegionSumCase(Row1: 4, Col1: 0, Row2: 4, Col2: 4, Expected: 9) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CreateByBruteForceCellScan_Cases_ReturnsRegionSum(RegionSumCase testCase)
    {
        var numMatrix = RangeSumQuery2DImmutableSolution.CreateByBruteForceCellScan(ExampleMatrix);
        var sum = numMatrix.SumRegion(testCase.Row1, testCase.Col1, testCase.Row2, testCase.Col2);

        Assert.Equal(testCase.Expected, sum);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CreateByPrefixSums_Cases_ReturnsRegionSum(RegionSumCase testCase)
    {
        var numMatrix = RangeSumQuery2DImmutableSolution.CreateByPrefixSums(ExampleMatrix);
        var sum = numMatrix.SumRegion(testCase.Row1, testCase.Col1, testCase.Row2, testCase.Col2);

        Assert.Equal(testCase.Expected, sum);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CreateByRowFenwickTree_Cases_ReturnsRegionSum(RegionSumCase testCase)
    {
        var numMatrix = RangeSumQuery2DImmutableSolution.CreateByRowFenwickTree(ExampleMatrix);
        var sum = numMatrix.SumRegion(testCase.Row1, testCase.Col1, testCase.Row2, testCase.Col2);

        Assert.Equal(testCase.Expected, sum);
    }

    // One case: the inclusive region as its top-left and bottom-right corners on the
    // shared ExampleMatrix, and the sum of the cells inside it. All four indices are bare
    // ints and two of them are rows, so each is named at every construction site and a
    // row reads as the case it is rather than as four positions a caller has to keep in
    // order. Nested because it is only ever used inside this test class - it is this
    // harness's own vocabulary, not a type another file would import.
    public readonly record struct RegionSumCase(int Row1, int Col1, int Row2, int Col2, int Expected);
}
