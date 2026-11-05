using DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BestTimeToBuyAndSellStockIV;

public sealed partial class BestTimeToBuyAndSellStockIVReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(BestTimeToBuyAndSellStockIVReach.IsWithin(1, new int[28]));

    [Fact]
    public void IsWithin_PricesLengthPastTheReach_IsFalse() => Assert.False(BestTimeToBuyAndSellStockIVReach.IsWithin(1, new int[29]));
}
