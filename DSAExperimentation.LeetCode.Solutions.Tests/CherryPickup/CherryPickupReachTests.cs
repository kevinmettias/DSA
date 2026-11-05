using DSAExperimentation.LeetCode.Solutions.CherryPickup;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CherryPickup;

public sealed partial class CherryPickupReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CherryPickupReach.IsWithin(new int[6, 6]));

    [Fact]
    public void IsWithin_RowCountPastTheReach_IsFalse() => Assert.False(CherryPickupReach.IsWithin(new int[7, 7]));
}
