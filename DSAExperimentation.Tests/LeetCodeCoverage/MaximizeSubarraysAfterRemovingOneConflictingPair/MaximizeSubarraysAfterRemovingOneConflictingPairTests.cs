using DSAExperimentation.LeetCode.MaximizeSubarraysAfterRemovingOneConflictingPair;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeSubarraysAfterRemovingOneConflictingPair;

// Harness only. Both strategies are
// MaximizeSubarraysAfterRemovingOneConflictingPairSolution's - this file just pins
// them to LeetCode's published examples.
public sealed partial class MaximizeSubarraysAfterRemovingOneConflictingPairTests
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
        int valueCount, int[][] conflictingPairs, int expected)
    {
        var actual = MaximizeSubarraysAfterRemovingOneConflictingPairSolution.MaxSubarraysByBruteForce(
            valueCount, conflictingPairs);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarraysByGroupedBoundSweep_LeetCodeExamples_ReturnsMaximumValidSubarrayCount(
        int valueCount, int[][] conflictingPairs, int expected)
    {
        var actual = MaximizeSubarraysAfterRemovingOneConflictingPairSolution.MaxSubarraysByGroupedBoundSweep(
            valueCount, conflictingPairs);

        Assert.Equal(expected, actual);
    }
}
