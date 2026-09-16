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

    public static TheoryData<RegionSumCase> Examples =>
        new()
        {
            { new RegionSumCase(Row1: 2, Col1: 1, Row2: 4, Col2: 3, Expected: 8) },
            { new RegionSumCase(Row1: 1, Col1: 1, Row2: 2, Col2: 2, Expected: 11) },
            { new RegionSumCase(Row1: 1, Col1: 2, Row2: 2, Col2: 4, Expected: 12) },
            { new RegionSumCase(Row1: 1, Col1: 1, Row2: 1, Col2: 1, Expected: 6) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBruteForceCellScan_LeetCodeExamples_ReturnsRegionSum(
        RegionSumCase example)
    {
        var numMatrix = RangeSumQuery2DImmutableSolution.CreateByBruteForceCellScan(ExampleMatrix);
        var sum = numMatrix.SumRegion(example.Row1, example.Col1, example.Row2, example.Col2);

        Assert.Equal(example.Expected, sum);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByRowFenwickTree_LeetCodeExamples_ReturnsRegionSum(
        RegionSumCase example)
    {
        var numMatrix = RangeSumQuery2DImmutableSolution.CreateByRowFenwickTree(ExampleMatrix);
        var sum = numMatrix.SumRegion(example.Row1, example.Col1, example.Row2, example.Col2);

        Assert.Equal(example.Expected, sum);
    }

    // One LeetCode example: the inclusive region as its top-left and bottom-right
    // corners on the shared ExampleMatrix, and the sum of the cells inside it. All
    // four indices are bare ints and two of them are rows, so each is named at every
    // construction site and a row reads as the case it is rather than as four
    // positions a caller has to keep in order. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct RegionSumCase(int Row1, int Col1, int Row2, int Col2, int Expected);
}
