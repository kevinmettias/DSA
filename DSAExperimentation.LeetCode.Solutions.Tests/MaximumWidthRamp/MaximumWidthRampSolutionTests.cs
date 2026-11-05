using DSAExperimentation.LeetCode.Solutions.MaximumWidthRamp;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumWidthRamp;

// Harness only. Both strategies are MaximumWidthRampSolution's - the pairwise baseline
// the benchmark used to hide, and the monotonically-decreasing candidate stack - so this
// file just pins them to the no-ramp case where the
// answer is 0 and the equal-values case where the widest ramp spans the whole array.
public sealed partial class MaximumWidthRampSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2, 2, 2, 2], 3 },
            { [1], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxWidthRampByPairwiseScan_Cases_ReturnsTheWidestRamp(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumWidthRampSolution.MaxWidthRampByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxWidthRampByCandidateStack_Cases_ReturnsTheWidestRamp(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumWidthRampSolution.MaxWidthRampByCandidateStack(nums));
}
