using DSAExperimentation.LeetCode.MaximumPrimeDifference;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumPrimeDifference;

// Harness only. Both strategies are MaximumPrimeDifferenceSolution's - this
// file just pins them to LeetCode's published examples, including the
// single-prime case whose answer is the degenerate distance 0.
public sealed class MaximumPrimeDifferenceTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [4, 2, 9, 5, 3], 3 },
            { [4, 8, 2, 8], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDistanceByBruteForcePairs_LeetCodeExamples_ReturnsWidestPrimeGap(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumPrimeDifferenceSolution.MaxDistanceByBruteForcePairs(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDistanceByEndpointScanWithSieve_LeetCodeExamples_ReturnsWidestPrimeGap(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumPrimeDifferenceSolution.MaxDistanceByEndpointScanWithSieve(nums));
}
