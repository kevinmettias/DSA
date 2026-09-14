using static DSAExperimentation.LeetCode.SpecialPermutations.SpecialPermutationsSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpecialPermutations;

// Harness only: both strategies are SpecialPermutationsSolution's, the same
// n!-enumeration baseline and (Remaining, Last) bitmask DP
// SpecialPermutationsBenchmarks measures.
public sealed class SpecialPermutationsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 3, 6], 2 }, // [3,6,2] and [2,6,3]
            { [1, 4, 3], 2 }, // [3,1,4] and [4,1,3] - 1 divides everything either way
            { [2, 4], 2 }, // [2,4] and [4,2] both valid: 4 % 2 == 0 regardless of order
            { [2, 3], 0 }, // neither divides the other, in either order
            { [3, 6, 12, 24], 24 }, // every pair divides, so all 4! orderings are special
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForceBacktracking_LeetCodeExamples_ReturnsSpecialPermutationCount(
        int[] nums, int expected)
        => Assert.Equal(expected, CountByBruteForceBacktracking(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBitmaskMemo_LeetCodeExamples_ReturnsSpecialPermutationCount(int[] nums, int expected)
        => Assert.Equal(expected, CountByBitmaskMemo(nums));
}
