using DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BestTimeToBuyAndSellStockII;

public sealed partial class BestTimeToBuyAndSellStockIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(BestTimeToBuyAndSellStockIIReach.IsWithin(new int[20]));

    [Fact]
    public void IsWithin_PricesLengthPastTheReach_IsFalse() => Assert.False(BestTimeToBuyAndSellStockIIReach.IsWithin(new int[21]));
}
