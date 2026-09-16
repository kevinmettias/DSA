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
    public void CountByBruteForce_LeetCodeExamples_ReturnsMatchingSubarrayCount(int[] nums, int targetValue, long expected)
    {
        var actual = NumberOfSubarraysWithANDValueOfKSolution.CountByBruteForce(nums, targetValue);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByAndValueCompression_LeetCodeExamples_ReturnsMatchingSubarrayCount(
        int[] nums, int targetValue, long expected)
    {
        var actual = NumberOfSubarraysWithANDValueOfKSolution.CountByAndValueCompression(nums, targetValue);

        Assert.Equal(expected, actual);
    }
}
