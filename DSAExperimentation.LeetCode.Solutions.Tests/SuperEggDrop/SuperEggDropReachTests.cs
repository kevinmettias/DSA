using DSAExperimentation.LeetCode.Solutions.SuperEggDrop;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SuperEggDrop;

public sealed partial class SuperEggDropReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SuperEggDropReach.IsWithin(1, 150));

    [Fact]
    public void IsWithin_FloorsPastTheReach_IsFalse() => Assert.False(SuperEggDropReach.IsWithin(1, 151));
}
