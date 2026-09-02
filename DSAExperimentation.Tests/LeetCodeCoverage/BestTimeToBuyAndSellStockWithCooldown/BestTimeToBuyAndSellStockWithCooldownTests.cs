using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockWithCooldown;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockWithCooldown;

// Harness only: the algorithms live in BestTimeToBuyAndSellStockWithCooldownSolution.
// One test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed class BestTimeToBuyAndSellStockWithCooldownTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 0, 2], 3 },
            { [1], 0 },
            { [1, 2, 4], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByUnmemoizedRecursion_LeetCodeExamples_ReturnsBestProfitWithCooldown(
        int[] prices, int expected) =>
        Assert.Equal(
            expected, BestTimeToBuyAndSellStockWithCooldownSolution.MaxProfitByUnmemoizedRecursion(prices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByMemoizedRecursion_LeetCodeExamples_ReturnsBestProfitWithCooldown(
        int[] prices, int expected) =>
        Assert.Equal(
            expected, BestTimeToBuyAndSellStockWithCooldownSolution.MaxProfitByMemoizedRecursion(prices));
}
