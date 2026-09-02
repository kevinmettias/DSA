using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStock;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStock;

// Harness only: both strategies live in BestTimeToBuyAndSellStockSolution and are
// asserted against the same examples.
public sealed class BestTimeToBuyAndSellStockTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [7, 1, 5, 3, 6, 4], 5 },
            { [7, 6, 4, 3, 1], 0 },
            { [1, 2], 1 },
            { [2], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByBruteForce_LeetCodeExamples_ReturnsBestSingleTrade(int[] prices, int expected) =>
        Assert.Equal(expected, BestTimeToBuyAndSellStockSolution.MaxProfitByBruteForce(prices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByOnePassMinTracking_LeetCodeExamples_ReturnsBestSingleTrade(int[] prices, int expected) =>
        Assert.Equal(expected, BestTimeToBuyAndSellStockSolution.MaxProfitByOnePassMinTracking(prices));
}
