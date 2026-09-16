using DSAExperimentation.LeetCode.KthSmallestAmountWithSingleDenominationCombination;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestAmountWithSingleDenominationCombination;

// Harness only. Both strategies are
// KthSmallestAmountWithSingleDenominationCombinationSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class KthSmallestAmountWithSingleDenominationCombinationTests
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
        int[] coins, long rank, long expected)
    {
        var actual = KthSmallestAmountWithSingleDenominationCombinationSolution.KthSmallestAmountByHeapMerge(coins, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestAmountByInclusionExclusionSearch_LeetCodeExamples_ReturnsKthAchievableAmount(
        int[] coins, long rank, long expected)
    {
        var actual = KthSmallestAmountWithSingleDenominationCombinationSolution.KthSmallestAmountByInclusionExclusionSearch(
            coins, rank);

        Assert.Equal(expected, actual);
    }
}
