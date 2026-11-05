using DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockWithTransactionFee;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BestTimeToBuyAndSellStockWithTransactionFee;

public sealed partial class BestTimeToBuyAndSellStockWithTransactionFeeReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(BestTimeToBuyAndSellStockWithTransactionFeeReach.IsWithin(new int[20], 100));

    [Fact]
    public void IsWithin_PricesLengthPastTheReach_IsFalse() => Assert.False(BestTimeToBuyAndSellStockWithTransactionFeeReach.IsWithin(new int[21], 100));

    [Fact]
    public void IsWithin_FeePastTheReach_IsFalse() => Assert.False(BestTimeToBuyAndSellStockWithTransactionFeeReach.IsWithin(new int[20], 101));
}
