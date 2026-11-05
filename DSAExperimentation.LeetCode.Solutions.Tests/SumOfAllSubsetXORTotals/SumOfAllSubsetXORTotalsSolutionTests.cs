using DSAExperimentation.LeetCode.Solutions.SumOfAllSubsetXORTotals;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumOfAllSubsetXORTotals;

// Harness only. Both the bitmask baseline and the Backtrack.Search walk are
// SumOfAllSubsetXORTotalsSolution's - this file just pins them to the
// single-element and duplicate-value edges, one theory per strategy so a failure
// names the arm that broke. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SumOfAllSubsetXORTotalsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [5], 5 },
            { [0], 0 },
            { [1, 1], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SubsetXorSumByBitmask_Cases_ReturnsSumOfAllSubsetTotals(int[] nums, int expected) =>
        Assert.Equal(expected, SumOfAllSubsetXORTotalsSolution.SubsetXorSumByBitmask(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SubsetXorSumByBacktracking_Cases_ReturnsSumOfAllSubsetTotals(int[] nums, int expected) =>
        Assert.Equal(expected, SumOfAllSubsetXORTotalsSolution.SubsetXorSumByBacktracking(nums));
}
