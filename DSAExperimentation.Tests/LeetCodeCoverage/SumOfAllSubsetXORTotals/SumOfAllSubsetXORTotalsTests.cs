using DSAExperimentation.LeetCode.SumOfAllSubsetXORTotals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfAllSubsetXORTotals;

// Harness only. Both the bitmask baseline and the Backtrack.Search walk are
// SumOfAllSubsetXORTotalsSolution's - this file just pins them to LeetCode's
// published examples plus the single-element and duplicate-value edges, one
// theory per strategy so a failure names the arm that broke.
public sealed partial class SumOfAllSubsetXORTotalsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 3], 6 },
            { [5, 1, 6], 28 },
            { [3, 4, 5, 6, 7, 8], 480 },
            { [5], 5 },
            { [0], 0 },
            { [1, 1], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubsetXorSumByBitmask_LeetCodeExamples_ReturnsSumOfAllSubsetTotals(int[] nums, int expected) =>
        Assert.Equal(expected, SumOfAllSubsetXORTotalsSolution.SubsetXorSumByBitmask(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubsetXorSumByBacktracking_LeetCodeExamples_ReturnsSumOfAllSubsetTotals(int[] nums, int expected) =>
        Assert.Equal(expected, SumOfAllSubsetXORTotalsSolution.SubsetXorSumByBacktracking(nums));
}
