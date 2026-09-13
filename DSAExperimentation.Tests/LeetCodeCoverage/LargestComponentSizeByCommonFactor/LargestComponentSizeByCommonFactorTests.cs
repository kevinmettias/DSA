using DSAExperimentation.LeetCode.LargestComponentSizeByCommonFactor;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestComponentSizeByCommonFactor;

// Harness only: both strategies live in LargestComponentSizeByCommonFactorSolution and
// are pinned to LeetCode's published examples, plus the degenerate cases the two arms
// have to agree on - a lone value, a pair sharing nothing, and a 1 (which has no prime
// factors at all, so it can never join a component).
public sealed class LargestComponentSizeByCommonFactorTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [4, 6, 15, 35], 4 },
            { [20, 50, 9, 63], 2 },
            { [2, 3, 6, 7, 4, 12, 21, 39], 8 },
            { [7], 1 },
            { [2, 3], 1 },
            { [1, 2, 3], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestComponentSizeByPairwiseGcd_LeetCodeExamples_ReturnsLargestSharedFactorComponent(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestComponentSizeByCommonFactorSolution.LargestComponentSizeByPairwiseGcd(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestComponentSizeByPrimeFactorUnion_LeetCodeExamples_ReturnsLargestSharedFactorComponent(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestComponentSizeByCommonFactorSolution.LargestComponentSizeByPrimeFactorUnion(nums));
}
