using DSAExperimentation.LeetCode.Solutions.SpecialPermutations;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SpecialPermutations;

// Harness only: both strategies are SpecialPermutationsSolution's, the same
// n!-enumeration baseline and (Remaining, Last) bitmask DP
// SpecialPermutationsBenchmarks measures.
public sealed partial class SpecialPermutationsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2, 4], 2 }, // [2,4] and [4,2] both valid: 4 % 2 == 0 regardless of order
            { [2, 3], 0 }, // neither divides the other, in either order
            { [3, 6, 12, 24], 24 }, // every pair divides, so all 4! orderings are special
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByBruteForceBacktracking_Cases_ReturnsSpecialPermutationCount(
        int[] nums, int expected)
        => Assert.Equal(expected, SpecialPermutationsSolution.CountByBruteForceBacktracking(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByBitmaskMemo_Cases_ReturnsSpecialPermutationCount(int[] nums, int expected)
        => Assert.Equal(expected, SpecialPermutationsSolution.CountByBitmaskMemo(nums));
}
