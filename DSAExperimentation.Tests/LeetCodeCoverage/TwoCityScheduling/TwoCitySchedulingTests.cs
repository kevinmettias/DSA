using DSAExperimentation.LeetCode.TwoCityScheduling;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoCityScheduling;

// Harness only: both strategies live in TwoCitySchedulingSolution and are asserted
// against the same examples - LeetCode's three published ones, plus the all-equal
// costs case where every split is optimal.
public sealed partial class TwoCitySchedulingTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[10, 20], [30, 200], [400, 50], [30, 20]], 110 },
            { [[259, 770], [448, 54], [926, 667], [184, 139], [840, 118], [577, 469]], 1859 },
            {
                [[515, 563], [451, 713], [537, 709], [343, 819], [855, 779], [457, 60], [650, 359], [631, 42]],
                3086
            },
            { [[1, 1], [1, 1]], 2 },
            { [[10, 20], [30, 200]], 50 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TwoCityCostMinimumByArraySortGreedy_LeetCodeExamples_ReturnsMinimumCost(
        int[][] costs, int expected) =>
        Assert.Equal(expected, TwoCitySchedulingSolution.TwoCityCostMinimumByArraySortGreedy(costs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TwoCityCostMinimumByMergeSortGreedy_LeetCodeExamples_ReturnsMinimumCost(
        int[][] costs, int expected) =>
        Assert.Equal(expected, TwoCitySchedulingSolution.TwoCityCostMinimumByMergeSortGreedy(costs));
}
