using DSAExperimentation.LeetCode.Solutions.TallestBillboard;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TallestBillboard;

public sealed partial class TallestBillboardReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(TallestBillboardReach.IsWithin(new int[9]));

    [Fact]
    public void IsWithin_RodsLengthPastTheReach_IsFalse() => Assert.False(TallestBillboardReach.IsWithin(new int[10]));
}
