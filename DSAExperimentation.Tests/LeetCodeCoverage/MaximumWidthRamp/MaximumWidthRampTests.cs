using DSAExperimentation.LeetCode.MaximumWidthRamp;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumWidthRamp;

// Harness only. Both strategies are MaximumWidthRampSolution's - the pairwise baseline
// the benchmark used to hide, and the monotonically-decreasing candidate stack - so this
// file just pins them to LeetCode's published examples plus the no-ramp case where the
// answer is 0 and the equal-values case where the widest ramp spans the whole array.
public sealed class MaximumWidthRampTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [6, 0, 8, 2, 1, 5], 4 },
            { [9, 8, 1, 0, 1, 9, 4, 0, 4, 1], 7 },
            { [5, 4, 3, 2, 1], 0 },
            { [2, 2, 2, 2], 3 },
            { [1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxWidthRampByPairwiseScan_LeetCodeExamples_ReturnsTheWidestRamp(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumWidthRampSolution.MaxWidthRampByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxWidthRampByCandidateStack_LeetCodeExamples_ReturnsTheWidestRamp(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumWidthRampSolution.MaxWidthRampByCandidateStack(nums));
}
