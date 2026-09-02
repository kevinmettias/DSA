using DSAExperimentation.LeetCode.KthSmallestAmountWithSingleDenominationCombination;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestAmountWithSingleDenominationCombination;

// Harness only. Both strategies are
// KthSmallestAmountWithSingleDenominationCombinationSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class KthSmallestAmountWithSingleDenominationCombinationTests
{
    public static TheoryData<int[], long, long> Examples =>
        new()
        {
            { [3, 6, 9], 3, 9 },
            { [5, 2], 7, 12 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestAmountByHeapMerge_LeetCodeExamples_ReturnsKthAchievableAmount(
        int[] coins, long k, long expected) =>
        Assert.Equal(
            expected,
            KthSmallestAmountWithSingleDenominationCombinationSolution.KthSmallestAmountByHeapMerge(coins, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestAmountByInclusionExclusionSearch_LeetCodeExamples_ReturnsKthAchievableAmount(
        int[] coins, long k, long expected) =>
        Assert.Equal(
            expected,
            KthSmallestAmountWithSingleDenominationCombinationSolution.KthSmallestAmountByInclusionExclusionSearch(
                coins, k));
}
