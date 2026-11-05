using DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockWithCooldown;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BestTimeToBuyAndSellStockWithCooldown;

public sealed partial class BestTimeToBuyAndSellStockWithCooldownReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(BestTimeToBuyAndSellStockWithCooldownReach.IsWithin(new int[28]));

    [Fact]
    public void IsWithin_PricesLengthPastTheReach_IsFalse() => Assert.False(BestTimeToBuyAndSellStockWithCooldownReach.IsWithin(new int[29]));
}
