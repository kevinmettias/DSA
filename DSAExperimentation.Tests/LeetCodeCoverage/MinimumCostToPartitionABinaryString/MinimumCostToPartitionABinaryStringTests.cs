using DSAExperimentation.LeetCode.MinimumCostToPartitionABinaryString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToPartitionABinaryString;

// Harness only: both strategies live in
// MinimumCostToPartitionABinaryStringSolution and are asserted against the
// same examples, so a failure names the strategy that broke.
public sealed class MinimumCostToPartitionABinaryStringTests
{
    public static TheoryData<string, int, int, long> Examples =>
        new()
        {
            { "1010", 2, 1, 6 },
            { "1010", 3, 10, 12 },
            { "00", 1, 2, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByLinearScanRecursion_LeetCodeExamples_ReturnsMinimumPartitionCost(
        string s, int encCost, int flatCost, long expected) =>
        Assert.Equal(expected, MinimumCostToPartitionABinaryStringSolution.MinCostByLinearScanRecursion(s, encCost, flatCost));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByFenwickRangeSum_LeetCodeExamples_ReturnsMinimumPartitionCost(
        string s, int encCost, int flatCost, long expected) =>
        Assert.Equal(expected, MinimumCostToPartitionABinaryStringSolution.MinCostByFenwickRangeSum(s, encCost, flatCost));
}
