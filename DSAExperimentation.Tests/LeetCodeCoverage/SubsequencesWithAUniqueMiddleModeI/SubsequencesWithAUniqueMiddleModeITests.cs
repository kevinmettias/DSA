using DSAExperimentation.LeetCode.SubsequencesWithAUniqueMiddleModeI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubsequencesWithAUniqueMiddleModeI;

// Harness only: both strategies are
// SubsequencesWithAUniqueMiddleModeISolution's. One test method per strategy
// over LeetCode's own examples, so a failure names the strategy that broke.
public sealed class SubsequencesWithAUniqueMiddleModeITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1, 1, 1, 1, 1, 1], 6 },
            { [1, 2, 2, 3, 3, 4], 4 },
            { [0, 1, 2, 3, 4, 5, 6, 7, 8], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMiddleModeSubsequencesByBruteForce_LeetCodeExamples_ReturnsUniqueMiddleModeCount(
        int[] nums, long expected) =>
        Assert.Equal(expected, SubsequencesWithAUniqueMiddleModeISolution.CountMiddleModeSubsequencesByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMiddleModeSubsequencesByModularCombinatorics_LeetCodeExamples_ReturnsUniqueMiddleModeCount(
        int[] nums, long expected) =>
        Assert.Equal(expected, SubsequencesWithAUniqueMiddleModeISolution.CountMiddleModeSubsequencesByModularCombinatorics(nums));
}
