using DSAExperimentation.LeetCode.FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximum;

// Harness only. Both strategies are
// FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1, 4, 3, 3, 2], 6 },
            { [3, 3, 3], 6 },
            { [1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsBoundaryMaxSubarrayCount(int[] nums, long expected) =>
        Assert.Equal(
            expected, FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumSolution.CountByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByMonotonicStack_LeetCodeExamples_ReturnsBoundaryMaxSubarrayCount(int[] nums, long expected) =>
        Assert.Equal(
            expected, FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumSolution.CountByMonotonicStack(nums));
}
