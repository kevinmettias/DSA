using DSAExperimentation.LeetCode.MaximumNumberOfOperationsWithTheSameScoreII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfOperationsWithTheSameScoreII;

// Harness only: both search strategies live in
// MaximumNumberOfOperationsWithTheSameScoreIISolution - this file just pins them
// to LeetCode's published examples.
public sealed partial class MaximumNumberOfOperationsWithTheSameScoreIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 2, 1, 2, 3, 4], 3 },
            { [3, 2, 6, 1, 4], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxOperationsByBruteForceDp_LeetCodeExamples_ReturnsMaxSameScoreOperationCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumNumberOfOperationsWithTheSameScoreIISolution.MaxOperationsByBruteForceDp(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxOperationsByMemoizedTwoPointer_LeetCodeExamples_ReturnsMaxSameScoreOperationCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected, MaximumNumberOfOperationsWithTheSameScoreIISolution.MaxOperationsByMemoizedTwoPointer(nums));
}
