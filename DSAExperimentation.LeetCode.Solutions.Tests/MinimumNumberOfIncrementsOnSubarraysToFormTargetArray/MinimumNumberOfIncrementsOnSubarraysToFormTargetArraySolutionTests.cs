using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfIncrementsOnSubarraysToFormTargetArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfIncrementsOnSubarraysToFormTargetArray;

// Harness only: both strategies live in
// MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution and are asserted against the
// same case - a flat array, where one stroke covers the whole target and no later rise can
// add to the count. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 1, 1, 1], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinNumberOperationsByLayerSimulation_Cases_ReturnsOperationCount(
        int[] target,
        int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution.MinNumberOperationsByLayerSimulation(target));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinNumberOperationsByRisingDiffScan_Cases_ReturnsOperationCount(
        int[] target,
        int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution.MinNumberOperationsByRisingDiffScan(target));
}
