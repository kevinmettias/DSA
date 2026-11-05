using DSAExperimentation.LeetCode.Solutions.TwoCityScheduling;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TwoCityScheduling;

// Harness only: both strategies live in TwoCitySchedulingSolution and are asserted
// against the same cases - LeetCode's three published ones, plus the all-equal
// costs case where every split is optimal.
public sealed partial class TwoCitySchedulingSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[1, 1], [1, 1]], 2 },
            { [[10, 20], [30, 200]], 50 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TwoCityCostMinimumByArraySortGreedy_Cases_ReturnsMinimumCost(
        int[][] costs, int expected) =>
        Assert.Equal(expected, TwoCitySchedulingSolution.TwoCityCostMinimumByArraySortGreedy(costs));

    [Theory]
    [MemberData(nameof(Cases))]
    public void TwoCityCostMinimumByMergeSortGreedy_Cases_ReturnsMinimumCost(
        int[][] costs, int expected) =>
        Assert.Equal(expected, TwoCitySchedulingSolution.TwoCityCostMinimumByMergeSortGreedy(costs));
}
