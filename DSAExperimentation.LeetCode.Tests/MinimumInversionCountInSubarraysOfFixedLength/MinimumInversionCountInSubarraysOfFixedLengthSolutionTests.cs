using DSAExperimentation.LeetCode.MinimumInversionCountInSubarraysOfFixedLength;

namespace DSAExperimentation.LeetCode.Tests.MinimumInversionCountInSubarraysOfFixedLength;

// Harness only. Both strategies are
// MinimumInversionCountInSubarraysOfFixedLengthSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class MinimumInversionCountInSubarraysOfFixedLengthSolutionTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [3, 1, 2, 5, 4], 3, 0 },
            { [5, 3, 2, 1], 4, 6 },
            { [2, 1], 1, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinInversionCountByBruteForce_LeetCodeExamples_ReturnsMinimumInversionCount(
        int[] nums, int windowLength, long expected)
    {
        var actual = MinimumInversionCountInSubarraysOfFixedLengthSolution.MinInversionCountByBruteForce(nums, windowLength);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinInversionCountBySlidingWindowFenwick_LeetCodeExamples_ReturnsMinimumInversionCount(
        int[] nums, int windowLength, long expected)
    {
        var actual = MinimumInversionCountInSubarraysOfFixedLengthSolution.MinInversionCountBySlidingWindowFenwick(nums, windowLength);

        Assert.Equal(expected, actual);
    }
}
