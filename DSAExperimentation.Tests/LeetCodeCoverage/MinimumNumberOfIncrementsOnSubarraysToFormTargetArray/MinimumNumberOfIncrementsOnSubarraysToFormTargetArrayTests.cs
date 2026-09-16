using DSAExperimentation.LeetCode.MinimumNumberOfIncrementsOnSubarraysToFormTargetArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfIncrementsOnSubarraysToFormTargetArray;

// Harness only: both strategies live in
// MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution and are asserted against the
// same examples - LeetCode's published ones plus a flat array, where one stroke covers the
// whole target and no later rise can add to the count.
public sealed partial class MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 2, 1], 3 },
            { [3, 1, 1, 2], 4 },
            { [3, 1, 5, 4, 2], 7 },
            { [1, 1, 1, 1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinNumberOperationsByLayerSimulation_LeetCodeExamples_ReturnsOperationCount(
        int[] target,
        int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution.MinNumberOperationsByLayerSimulation(target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinNumberOperationsByRisingDiffScan_LeetCodeExamples_ReturnsOperationCount(
        int[] target,
        int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution.MinNumberOperationsByRisingDiffScan(target));
}
