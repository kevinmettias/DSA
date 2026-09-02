using DSAExperimentation.LeetCode.LexicographicallyMaximumMEXArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicallyMaximumMEXArray;

// Harness only. Both cutting strategies are
// LexicographicallyMaximumMEXArraySolution's - this file just pins them to
// LeetCode's published examples.
public sealed class LexicographicallyMaximumMEXArrayTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [0, 1, 0], [2, 1] },
            { [1, 0, 2], [3] },
            { [3, 1], [0, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MexArrayByBruteForce_LeetCodeExamples_ReturnsLexicographicallyMaximumArray(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, LexicographicallyMaximumMEXArraySolution.MexArrayByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MexArrayByFrequencyPointer_LeetCodeExamples_ReturnsLexicographicallyMaximumArray(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, LexicographicallyMaximumMEXArraySolution.MexArrayByFrequencyPointer(nums));
}
