using DSAExperimentation.LeetCode.MinimumOperationsToConvertAllElementsToZero;

namespace DSAExperimentation.LeetCode.Tests.MinimumOperationsToConvertAllElementsToZero;

// Harness only. The range-minimum decomposition both strategies share is
// MinimumOperationsToConvertAllElementsToZeroSolution's - this file just pins them
// to LeetCode's published examples.
public sealed partial class MinimumOperationsToConvertAllElementsToZeroSolutionTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [0, 2], 1 },
            { [3, 1, 2, 1], 3 },
            { [1, 2, 1, 2, 1, 2], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByDivideAndConquer_LeetCodeExamples_ReturnsMinimumOperationCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, MinimumOperationsToConvertAllElementsToZeroSolution.MinOperationsByDivideAndConquer(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByMonotonicStack_LeetCodeExamples_ReturnsMinimumOperationCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, MinimumOperationsToConvertAllElementsToZeroSolution.MinOperationsByMonotonicStack(nums));
}
