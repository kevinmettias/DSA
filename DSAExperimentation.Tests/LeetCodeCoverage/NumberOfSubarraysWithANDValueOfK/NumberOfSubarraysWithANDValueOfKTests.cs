using DSAExperimentation.LeetCode.NumberOfSubarraysWithANDValueOfK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfSubarraysWithANDValueOfK;

// Harness only. Both strategies are
// NumberOfSubarraysWithANDValueOfKSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class NumberOfSubarraysWithANDValueOfKTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, 1, 1], 1, 6 },
            { [1, 1, 2], 1, 3 },
            { [1, 2, 3], 2, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsMatchingSubarrayCount(int[] nums, int k, long expected) =>
        Assert.Equal(expected, NumberOfSubarraysWithANDValueOfKSolution.CountByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByAndValueCompression_LeetCodeExamples_ReturnsMatchingSubarrayCount(int[] nums, int k, long expected) =>
        Assert.Equal(expected, NumberOfSubarraysWithANDValueOfKSolution.CountByAndValueCompression(nums, k));
}
