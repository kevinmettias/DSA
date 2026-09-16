using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockUsingStrategy;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockUsingStrategy;

// Harness only. Both window-scoring strategies are
// BestTimeToBuyAndSellStockUsingStrategySolution's - this file just pins them to
// LeetCode's published examples.
public sealed class BestTimeToBuyAndSellStockUsingStrategyTests
{
    public static TheoryData<int[], int[], int, long> Examples =>
        new()
        {
            { [4, 2, 8], [-1, 0, 1], 2, 10 },
            { [5, 4, 3], [1, 1, 0], 2, 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByBruteForceWindowSum_LeetCodeExamples_ReturnsBestAchievableProfit(
        int[] prices, int[] strategy, int k, long expected)
    {
        var actual = BestTimeToBuyAndSellStockUsingStrategySolution.MaxProfitByBruteForceWindowSum(
            prices, strategy, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitBySlidingWindowSum_LeetCodeExamples_ReturnsBestAchievableProfit(
        int[] prices, int[] strategy, int k, long expected)
    {
        var actual = BestTimeToBuyAndSellStockUsingStrategySolution.MaxProfitBySlidingWindowSum(
            prices, strategy, k);

        Assert.Equal(expected, actual);
    }
}
