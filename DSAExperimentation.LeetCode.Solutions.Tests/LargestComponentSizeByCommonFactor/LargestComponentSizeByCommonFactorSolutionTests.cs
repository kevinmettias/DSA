using DSAExperimentation.LeetCode.Solutions.LargestComponentSizeByCommonFactor;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LargestComponentSizeByCommonFactor;

// Harness only: both strategies live in LargestComponentSizeByCommonFactorSolution and
// are pinned to the degenerate cases the two arms
// have to agree on - a lone value, a pair sharing nothing, and a 1 (which has no prime
// factors at all, so it can never join a component).
public sealed partial class LargestComponentSizeByCommonFactorSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [7], 1 },
            { [2, 3], 1 },
            { [1, 2, 3], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestComponentSizeByPairwiseGcd_Cases_ReturnsLargestSharedFactorComponent(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestComponentSizeByCommonFactorSolution.LargestComponentSizeByPairwiseGcd(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestComponentSizeByPrimeFactorUnion_Cases_ReturnsLargestSharedFactorComponent(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestComponentSizeByCommonFactorSolution.LargestComponentSizeByPrimeFactorUnion(nums));
}
