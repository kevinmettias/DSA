using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockII;

// Harness only: both strategies live in BestTimeToBuyAndSellStockIISolution and
// are asserted against the same examples.
public sealed class BestTimeToBuyAndSellStockIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [7, 1, 5, 3, 6, 4], 7 },
            { [1, 2, 3, 4, 5], 4 },
            { [7, 6, 4, 3, 1], 0 },
            { [1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByBruteForce_LeetCodeExamples_ReturnsAllUpwardGains(int[] prices, int expected) =>
        Assert.Equal(expected, BestTimeToBuyAndSellStockIISolution.MaxProfitByBruteForce(prices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByGreedyAscent_LeetCodeExamples_ReturnsAllUpwardGains(int[] prices, int expected) =>
        Assert.Equal(expected, BestTimeToBuyAndSellStockIISolution.MaxProfitByGreedyAscent(prices));
}
