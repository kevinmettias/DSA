using DSAExperimentation.LeetCode.MinimumCostToPartitionABinaryString;

namespace DSAExperimentation.LeetCode.Tests.MinimumCostToPartitionABinaryString;

// Harness only: both strategies live in
// MinimumCostToPartitionABinaryStringSolution and are asserted against the
// same examples, so a failure names the strategy that broke.
public sealed partial class MinimumCostToPartitionABinaryStringSolutionTests
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
        string binaryString, int encCost, int flatCost, long expected)
    {
        var actual = MinimumCostToPartitionABinaryStringSolution.MinCostByLinearScanRecursion(
            binaryString, encCost, flatCost);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByFenwickRangeSum_LeetCodeExamples_ReturnsMinimumPartitionCost(
        string binaryString, int encCost, int flatCost, long expected)
    {
        var actual = MinimumCostToPartitionABinaryStringSolution.MinCostByFenwickRangeSum(
            binaryString, encCost, flatCost);

        Assert.Equal(expected, actual);
    }
}
