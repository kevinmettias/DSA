using DSAExperimentation.LeetCode.MaximizeSubarraysAfterRemovingOneConflictingPair;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeSubarraysAfterRemovingOneConflictingPair;

// Harness only. Both strategies are
// MaximizeSubarraysAfterRemovingOneConflictingPairSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class MaximizeSubarraysAfterRemovingOneConflictingPairTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            { 4, [[2, 3], [1, 4]], 9 },
            { 5, [[1, 2], [2, 5], [3, 5]], 12 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarraysByBruteForce_LeetCodeExamples_ReturnsMaximumValidSubarrayCount(
        int n, int[][] conflictingPairs, int expected) =>
        Assert.Equal(
            expected,
            MaximizeSubarraysAfterRemovingOneConflictingPairSolution.MaxSubarraysByBruteForce(n, conflictingPairs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarraysByGroupedBoundSweep_LeetCodeExamples_ReturnsMaximumValidSubarrayCount(
        int n, int[][] conflictingPairs, int expected) =>
        Assert.Equal(
            expected,
            MaximizeSubarraysAfterRemovingOneConflictingPairSolution.MaxSubarraysByGroupedBoundSweep(n, conflictingPairs));
}
