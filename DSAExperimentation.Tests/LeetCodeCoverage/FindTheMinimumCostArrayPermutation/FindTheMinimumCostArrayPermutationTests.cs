using DSAExperimentation.LeetCode.FindTheMinimumCostArrayPermutation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheMinimumCostArrayPermutation;

// Harness only. Both strategies live in FindTheMinimumCostArrayPermutationSolution -
// this file just pins them to LeetCode's published examples.
public sealed partial class FindTheMinimumCostArrayPermutationTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 0, 2], [0, 1, 2] },
            { [0, 2, 1], [0, 2, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPermutationByBruteForceSearch_LeetCodeExamples_ReturnsLexicographicallySmallestMinimumCostPermutation(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, FindTheMinimumCostArrayPermutationSolution.FindPermutationByBruteForceSearch(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPermutationByBitmaskMemoization_LeetCodeExamples_ReturnsLexicographicallySmallestMinimumCostPermutation(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, FindTheMinimumCostArrayPermutationSolution.FindPermutationByBitmaskMemoization(nums));
}
