using DSAExperimentation.LeetCode.Solutions.BurstBalloons;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BurstBalloons;

public sealed partial class BurstBalloonsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(BurstBalloonsReach.IsWithin(new int[12]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(BurstBalloonsReach.IsWithin(new int[13]));
}
