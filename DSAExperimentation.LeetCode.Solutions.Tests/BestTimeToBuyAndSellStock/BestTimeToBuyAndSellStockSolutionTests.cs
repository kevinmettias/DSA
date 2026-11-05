using DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStock;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BestTimeToBuyAndSellStock;

// Harness only: both strategies live in BestTimeToBuyAndSellStockSolution and are
// asserted against the same cases.
public sealed partial class BestTimeToBuyAndSellStockSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProfitByBruteForce_Cases_ReturnsBestSingleTrade(int[] prices, int expected) =>
        Assert.Equal(expected, BestTimeToBuyAndSellStockSolution.MaxProfitByBruteForce(prices));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProfitByOnePassMinTracking_Cases_ReturnsBestSingleTrade(int[] prices, int expected) =>
        Assert.Equal(expected, BestTimeToBuyAndSellStockSolution.MaxProfitByOnePassMinTracking(prices));
}
