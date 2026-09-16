using DSAExperimentation.LeetCode.NumberOfEffectiveSubsequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfEffectiveSubsequences;

// Harness only. Both strategies are NumberOfEffectiveSubsequencesSolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class NumberOfEffectiveSubsequencesTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3], 3 },
            { [7, 4, 6], 4 },
            { [8, 8], 1 },
            { [2, 2, 1], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountEffectiveByBruteForce_LeetCodeExamples_ReturnsEffectiveSubsequenceCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfEffectiveSubsequencesSolution.CountEffectiveByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountEffectiveByOrSubsetTransform_LeetCodeExamples_ReturnsEffectiveSubsequenceCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfEffectiveSubsequencesSolution.CountEffectiveByOrSubsetTransform(nums));
}
