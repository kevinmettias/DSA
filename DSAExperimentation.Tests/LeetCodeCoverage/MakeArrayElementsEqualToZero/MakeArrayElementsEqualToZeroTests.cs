using DSAExperimentation.LeetCode.MakeArrayElementsEqualToZero;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MakeArrayElementsEqualToZero;

// Harness only: both strategies live in MakeArrayElementsEqualToZeroSolution.
// One test method per strategy over one shared set of LeetCode's own
// examples, so a failure names the strategy that broke.
public sealed partial class MakeArrayElementsEqualToZeroTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 0, 2, 0, 3], 2 },
            { [2, 3, 4, 0, 4, 1, 0], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountValidSelectionsByBruteForceSimulation_LeetCodeExamples_ReturnsValidSelectionCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, MakeArrayElementsEqualToZeroSolution.CountValidSelectionsByBruteForceSimulation(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountValidSelectionsByPrefixSumBalance_LeetCodeExamples_ReturnsValidSelectionCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, MakeArrayElementsEqualToZeroSolution.CountValidSelectionsByPrefixSumBalance(nums));
}
