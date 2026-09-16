using DSAExperimentation.LeetCode.CountBowlSubarrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountBowlSubarrays;

// Harness only. Both strategies are CountBowlSubarraysSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class CountBowlSubarraysTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 5, 3, 1, 4], 2 },
            { [5, 1, 2, 3, 4], 3 },
            { [1_000_000_000, 999_999_999, 999_999_998], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBowlsByPairScan_LeetCodeExamples_ReturnsBowlSubarrayCount(int[] nums, int expected) =>
        Assert.Equal(expected, CountBowlSubarraysSolution.CountBowlsByPairScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBowlsByMonotonicStack_LeetCodeExamples_ReturnsBowlSubarrayCount(int[] nums, int expected) =>
        Assert.Equal(expected, CountBowlSubarraysSolution.CountBowlsByMonotonicStack(nums));
}
